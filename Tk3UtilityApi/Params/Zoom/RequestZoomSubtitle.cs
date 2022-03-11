using System.ComponentModel.DataAnnotations;

namespace Tk3UtilityApi.Params.Zoom
{
    public class RequestZoomSubtitle
    {
        [Required]
        public int Sequence { get; init; }

        [Required]
        public string Message { get; init; } = null!;
    }
}
