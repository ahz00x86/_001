using Microsoft.AspNetCore.Mvc;
using TokensWay.Data.pDomino;

namespace TokensWay.Controllers
{
    public class pDominoC : Controller
    {
        private readonly pDomino _pDomino;

        public pDominoC(pDomino pDomino)
        {
            _pDomino = pDomino;
        }


        public IActionResult pDomino()
        {
            return View();
        }
    }
}
