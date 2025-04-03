using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DataAccess.Repositories
{
    public class PollFileRepository : IPollRepository
    {

        private string _fileName;

        /// <summary>
        /// This constructor initializes the PollFileRepository with the configuration and environment.
        /// It gets the file name from the configuration.
        /// Then it checks if the file exists, if not it creates it with an empty JSON array, and saves the file in the root directory of the project.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PollFileRepository(IConfiguration configuration, IWebHostEnvironment env)
        {
            var fileNameFromConfigFile = configuration["PollsFileName"];
            if(string.IsNullOrEmpty(fileNameFromConfigFile))
            {
                throw new ArgumentNullException("PollsFileName is not defined in the config file");
            }

            _fileName = Path.Combine(env.ContentRootPath, fileNameFromConfigFile!);

            if (!System.IO.File.Exists(_fileName))
            {
                System.IO.File.WriteAllText(_fileName, "[]");
            }
        }

        /// <summary>
        /// This method creates a new poll in the polls.json file.
        /// It first finds all the polls in the file, adds the new poll to the list, and then saves the list back to the file.
        /// </summary>
        /// <param name="poll">The Poll object that is to be saved to polls.json</param>
        public bool CreatePoll(Poll poll)
        {
            if (poll == null)
            {
                return false;
            }

            var polls = GetPolls().ToList();
            poll.Id = Guid.NewGuid();
            polls.Add(poll);
            string json = JsonConvert.SerializeObject(polls);
            System.IO.File.WriteAllText(_fileName, json);
            return true;
        }

        /// <summary>
        /// This method gets all the polls from the polls.json file.
        /// If there are no polls, it returns an empty list.
        /// Otherwise, it deserializes the JSON file into a list of Poll objects and returns it as an IQueryable.
        /// </summary>
        /// <returns>The List of polls retrieved from polls.json</returns>
        public IQueryable<Poll> GetPolls()
        {
            if (!System.IO.File.Exists(_fileName))
            {
                return new List<Poll>().AsQueryable();
            }

            string contents = System.IO.File.ReadAllText(_fileName);

            var polls = JsonConvert.DeserializeObject<List<Poll>>(contents) ?? new List<Poll>();

            return polls.AsQueryable();
        }
    }
}
