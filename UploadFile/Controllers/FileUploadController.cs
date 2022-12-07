using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.VisualBasic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UploadFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadController(IWebHostEnvironment webHostEnvironment)
        {
           _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/<ValuesController>
        [HttpPost("[action]")]
        public IActionResult UploadFiles(List<IFormFile> files)
        {
            if (files.Count == 0)
                return BadRequest();
            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");

            foreach (var file in files)
            {
                string filePath = Path.Combine(directoryPath, file.FileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            } 
            return Ok("Upload Succesful");
        }

        [HttpGet("[action]")]
        public IActionResult LoadFile()
        {
            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");

            List<string> fileResults = new List<string>();

            foreach (string filepath in Directory.EnumerateFiles(directoryPath))
            {
                string result = HandleFile(filepath);
                fileResults.Add(result);
            }

            return Ok(fileResults);

        }   
        private string HandleFile(string file)
        {
            string contents = System.IO.File.ReadAllText(file);

            var contentSplitted = contents.Split(' ');

            var listOfWords = contentSplitted.Where(currentWord => !string.IsNullOrEmpty(currentWord));

            string? mostUsedWord = FindMostUsedWord(listOfWords);

            string result = "";

            foreach (var currentWord in contentSplitted)
            {
                result = result + HandleWord(currentWord, mostUsedWord);
            }
            return result;
        }
        
        private string HandleWord(string currentWord, string? mostUsedWord)
        {
            string foo = "foo";
            string bar = "bar";

            if (currentWord.ToLower() == mostUsedWord?.ToLower())
            {
                string sewingTogetherWord = string.Concat(foo, currentWord, bar);
                return sewingTogetherWord;
            }
            else
            {
                return currentWord;
            }
        }
        private string? FindMostUsedWord(IEnumerable<string> words)
        {
            var groupedWords = words.GroupBy(s => s);
            var filtered = groupedWords.Where(x => x.Count() > 0);
            var orderedBy = filtered.OrderByDescending(x => x.Count());
            var select = orderedBy.Select(x => x.Key);

            var mostUsedWord = select.FirstOrDefault();

            return mostUsedWord;
        }
    }
}
