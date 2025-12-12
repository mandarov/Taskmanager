using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskPlanner.Api.Data;
using TaskPlanner.Api.Models;

namespace TaskPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly TaskDbContext _context;

        public TasksController(TaskDbContext context)
        {
            _context = context;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks([FromQuery] bool? completed = null)
        {
            var query = _context.Tasks.AsQueryable();

            if (completed.HasValue)
            {
                query = query.Where(t => t.IsCompleted == completed.Value);
            }

            var tasks = await query.OrderBy(t => t.CreatedDate).ToListAsync();
            return Ok(tasks);
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                return BadRequest("Title is required");
            }

            task.CreatedDate = DateTime.UtcNow;
            task.IsCompleted = false;
            task.CompletedDate = null;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task); // ИСПРАВЛЕНО: task.Id
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem task)
        {
            if (id != task.Id) // ИСПРАВЛЕНО: task.Id
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(task.Title))
            {
                return BadRequest("Title is required");
            }

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;

            if (!existingTask.IsCompleted && task.IsCompleted)
            {
                existingTask.IsCompleted = true;
                existingTask.CompletedDate = DateTime.UtcNow;
            }
            else if (existingTask.IsCompleted && !task.IsCompleted)
            {
                existingTask.IsCompleted = false;
                existingTask.CompletedDate = null;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}