using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class UiController : Controller
    {
        [HttpGet] public async Task<IActionResult> Accordions() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> AdvanceSwiper() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Alerts() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Avatars() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Badges() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Block() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Breadcrumbs() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> ButtonGroup() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Buttons() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Card() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Carousel() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Cookie() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Countup() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> DatePicker() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> DraggableCards() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Dropdowns() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> FloatingLabels() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> ImagesFigures() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Links() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> List() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> MediaPlayer() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Modal() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Offcanvas() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Pagination() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Placeholders() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Popover() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Progress() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Ratings() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Ribbons() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Scrollspy() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Separator() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> SortableJs() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Spinner() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Sweetalert2() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Tabs() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Toast() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Tooltips() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Tour() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Treeview() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Typography() => await Task.FromResult(View());
        [HttpGet] public async Task<IActionResult> Utilities() => await Task.FromResult(View());
    }
}