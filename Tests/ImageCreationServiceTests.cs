using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageCreatorWebAPI;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageCreatorWebApiTests
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly byte[] _response;
        private readonly HttpStatusCode _statusCode;

        public string Input { get; private set; }
        public int NumberOfCalls { get; private set; }

        public MockHttpMessageHandler(byte[] response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            NumberOfCalls++;
            if (request.Content != null) // Could be a GET-request without a body
            {
                Input = await request.Content.ReadAsStringAsync();
            }
            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new ByteArrayContent(_response)
            };
        }
    }
    
    [TestClass]
    public class ImageCreationServiceTests
    {
        private ImageCreationService _imageCreationService;
        private MockHttpMessageHandler _mockHttpMessageHandler;
        
        [TestInitialize]
        public void TestInitialize()
        {
            var bitmap = new Image<Rgba32>(256, 256);
            var stream = new MemoryStream();
            bitmap.SaveAsPng(stream);
            var factory = Substitute.For<IHttpClientFactory>();
            _mockHttpMessageHandler = new MockHttpMessageHandler(stream.ToArray(), HttpStatusCode.OK);
            factory.CreateClient().Returns(_ =>new HttpClient(_mockHttpMessageHandler));
            _imageCreationService = new ImageCreationService(factory, Substitute.For<ILogger<ImageCreationService>>());
        }

        private DataContainer GetDataContainer(List<LatLngTime> latLngs)
        {
            return new DataContainer
            {
                BaseLayer = new LayerData(),
                Routes = new List<RouteData>
                {
                    new RouteData
                    {
                        Segments = new List<RouteSegmentData>
                        {
                            new RouteSegmentData
                            {
                                Latlngs = latLngs
                            }
                        },
                        Markers = new List<MarkerData>
                        {
                            new MarkerData {Latlng = latLngs.FirstOrDefault() }
                        }
                    }
                }
            };
        }

        [TestMethod]
        public void Zoom16_RouteInSingleTile_ShouldResizeSingleTile6Times()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.0001, Lng = 0.0001},
                new LatLngTime {Lat = 0.0002, Lng = 0.0002}
            });

            var results = _imageCreationService.Create(dataContainer, 512, 256).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 6);
        }

        [TestMethod]
        public void Zoom16_RouteInSingleTileTileIsMissing_ShouldUseEmptyBitmaps()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.0001, Lng = 0.0001},
                new LatLngTime {Lat = 0.0002, Lng = 0.0002}
            });
            // HM TODO
            //_remoteFileFetcherGateway.GetFileContent(Arg.Any<string>())
            //    .Returns(new RemoteFileFetcherGatewayResponse
            //    {
            //        FileName = "missing.png",
            //        Content = new byte[0]
            //    });

            var results = _imageCreationService.Create(dataContainer, 600, 255).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 8);
        }

        [TestMethod]
        public void Zoom16_RouteInTwoHorizontalTiles_ShouldResize4Tile3Times()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.01, Lng = 0.01},
                new LatLngTime {Lat = 0.01, Lng = 0.015}
            });

            var results = _imageCreationService.Create(dataContainer, 512, 256).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 12);
        }

        [TestMethod]
        public void Zoom16_RouteInTwoVerticalTile_ShouldResize5Tile3Times()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.01, Lng = 0.01},
                new LatLngTime {Lat = 0.015, Lng = 0.01}
            });

            var results = _imageCreationService.Create(dataContainer, 512, 256).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 15);        }

        [TestMethod]
        public void Zoom13_RouteInSingleTile_UseZoom13Tiles()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.1, Lng = 0.1},
                new LatLngTime {Lat = 0.15, Lng = 0.15}
            });

            var results = _imageCreationService.Create(dataContainer, 600, 255).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 8);        }

        [TestMethod]
        public void Zoom13_RouteIsNarrowHorizontalLine_UseZoom14Tiles()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.1, Lng = 0.1},
                new LatLngTime {Lat = 0.1, Lng = 0.15}
            });

            var results = _imageCreationService.Create(dataContainer, 512, 256).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 9);        }

        [TestMethod]
        public void Zoom13_RouteIsNarrowVerticalLine_ShouldUseZoom13Tiles()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.1, Lng = 0.1},
                new LatLngTime {Lat = 0.15, Lng = 0.1}
            });

            var results = _imageCreationService.Create(dataContainer, 600, 255).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 8);
            //_remoteFileFetcherGateway.Received(8).GetFileContent(Arg.Is<string>(x => x.Contains("/13/")));
        }

        [TestMethod]
        public void Zoom13_RouteIsNarrowVerticalLineWithOverlay_ShouldFetchOverlayTiles()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.1, Lng = 0.1},
                new LatLngTime {Lat = 0.15, Lng = 0.1}
            });
            dataContainer.Overlays = new List<LayerData>{ new LayerData { Address = "http://www.overlay.com/{z}/{y}/{x}" } };

            var results = _imageCreationService.Create(dataContainer, 600, 255).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 16);
        }

        [TestMethod]
        public void Zoom13_OverlayIsNotInAValidFormat_ShouldFetchOnlyBaseLayer()
        {
            var dataContainer = GetDataContainer(new List<LatLngTime>
            {
                new LatLngTime {Lat = 0.1, Lng = 0.1},
                new LatLngTime {Lat = 0.15, Lng = 0.1}
            });
            dataContainer.Overlays = new List<LayerData> { new LayerData { Address = "overlay" } };

            var results = _imageCreationService.Create(dataContainer, 600, 255).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 8);
        }

        [TestMethod]
        public void LocalTiles_RouteWithNoPoints_ShouldReturnBackgroundImageFromBounds()
        {
            var dataContainer = new DataContainer
            {
                NorthEast = new LatLng { Lat = 0.15, Lng = 0.15 },
                SouthWest = new LatLng { Lat = 0.1, Lng = 0.1 },
                BaseLayer = new LayerData { Address = "/Tiles/{z}/{x}/{y}.png"},
                Routes = new List<RouteData> {  new RouteData() }
            };

            var results = _imageCreationService.Create(dataContainer, 600, 255).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 8);
        }

        [TestMethod]
        public void LocalTiles_RouteColorAndOpacity_ShouldDrawAccordingly()
        {
            var dataContainer = new DataContainer
            {
                BaseLayer = new LayerData { Address = "/Tiles/{z}/{x}/{y}.png" },
                Overlays = new List<LayerData>
                {
                    new LayerData(), // should be ignored
                    new LayerData { Address = "http://www.address.com/{z}/{y}/{x}.png", Opacity = 0.5 }
                },
                Routes = new List<RouteData> { new RouteData
                {
                    Color = "red",
                    Opacity = 0.5,
                    Segments = new List<RouteSegmentData>
                    {
                        new RouteSegmentData {Latlngs = new List<LatLngTime>
                        {
                            new LatLngTime { Lat = 0.15, Lng = 0.15 },
                            new LatLngTime { Lat = 0.1, Lng = 0.1 }
                        }}
                    }
                } }
            };

            var results = _imageCreationService.Create(dataContainer, 600, 255).Result;

            Assert.IsTrue(results.Length > 0);
            Assert.AreEqual(_mockHttpMessageHandler.NumberOfCalls, 16);
        }
    }
}