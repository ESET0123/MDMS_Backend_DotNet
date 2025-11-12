using MDMS_Backend.Models;
using MDMS_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MDMS_Backend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class RoleController : Controller
    {
        private readonly IRoleRepository _rolerepo;

        public RoleController(IRoleRepository roleRepo)
        {
            _rolerepo = roleRepo;
        }

        [HttpGet]
        [Route("AllRoles")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<List<Role>>> GetRoles()
        {
            var roles = await _rolerepo.GetAllAsync();
            return Ok(roles);
        }

            
        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<int>> CreateRole([FromBody] RoleDTO model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            Role roleNew = new Role
            {
                RoleName = model.RoleName,
                Abbreviation = model.Abbreviation
            };
            await _rolerepo.AddAsync(roleNew);
            return Ok();
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateRole([FromBody] RoleDTO model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var existingRole = await _rolerepo.GetByIdAsync(model.RoleId);
            if (existingRole == null)
            {
                return NotFound();
            }

            Role roleUpdate = new Role
            {
                RoleName = model.RoleName,
                Abbreviation = model.Abbreviation
            };

            await _rolerepo.UpdateAsync(roleUpdate);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteRole(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            await _rolerepo.DeleteAsync(id);
            return Ok();
        }
    }

    public class RoleDTO
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public string Abbreviation { get; set; } = null!;
    }
}
