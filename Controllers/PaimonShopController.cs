using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaimonShopApi.Data;

namespace PaimonShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaimonShopController : ControllerBase
    {
        private readonly DataContext context;

        public PaimonShopController(DataContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(context.PaimonShops);
        }

        [HttpGet("{id}")]

        public ActionResult Get(int id)
        {
            var branch = context.PaimonShops.Find(id);
            if (branch == null) {
                return BadRequest("Don't have Branch right now");
            }
            return Ok(branch);
        }

        [HttpPost]
        // public ActionResult<List<PaimonShop>> AddBranch(PaimonShop branch)
        // {
        //     paimonshop.Add(branch);
        //     return Ok(paimonshop);
        // }

        public ActionResult Create(PaimonShop branch)
        {
           context.PaimonShops.Add(branch);
           context.SaveChanges();
           return Ok("xong");
        }


        [HttpPut]
        public ActionResult UpdateBranch(PaimonShop request)
        {
            var branch = context.PaimonShops.FirstOrDefault(x => x.Id == request.Id);
            if (branch == null) {
                return BadRequest("Don't have Branch right now");
            }
            branch.ShopName = request.ShopName;
            branch.Address = request.Address;
            branch.ShopItem = request.ShopItem;
            branch.Email = request.Email;
            context.SaveChanges();
            return Ok("Được rồi");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var branch = context.PaimonShops.FirstOrDefault(x => x.Id == id);
            if (branch == null) {
                return BadRequest("Does not esist branch");
            }
            context.PaimonShops.Remove(branch);
            context.SaveChanges();
            return Ok("Deleted");
        }
    }
}