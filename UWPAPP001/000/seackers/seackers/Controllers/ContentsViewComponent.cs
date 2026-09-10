using Microsoft.AspNetCore.Mvc;

namespace seackers.Controllers
{
    public class ContentsViewComponent : ViewComponent
    {
        private readonly UserController _userController;

        public ContentsViewComponent(UserController userController)
        {
            _userController = userController;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contents = await _userController.GetContents();
            return View(contents);
        }
    }
}
