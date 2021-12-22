using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ImageCreatorWebAPI
{
    /// <summary>
    /// A controller to create images
    /// </summary>
    [ApiController]
    [Route("")]
    public class ImageCreationController : ControllerBase
    {
        private readonly ImageCreationService _imageCreationService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imageCreationService"></param>
        public ImageCreationController(ImageCreationService imageCreationService)
        {
            _imageCreationService = imageCreationService;
        }

        /// <summary>
        /// When sending a <see cref="DataContainer"/> you'll receive the image preview
        /// </summary>
        /// <param name="dataContainer">The container to create the image for</param>
        /// <param name="width">[Optional] Image width, default 600</param>
        /// <param name="height">[Optional] Image height, default 315</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PostDataContainer([FromBody]DataContainer dataContainer, [FromQuery]int? width, [FromQuery]int? height)
        {
            var imageData = await _imageCreationService.Create(dataContainer, width ?? 600, height ?? 315);
            return new FileContentResult(imageData, new MediaTypeHeaderValue("image/png"));
        }
    }
}