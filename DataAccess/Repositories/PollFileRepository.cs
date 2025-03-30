using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PollFileRepository : IPollRepository
    {

        private string _fileName;

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
                System.IO.File.WriteAllText(_fileName, "[]"); // empty JSON array
            }
        }

        public void CreatePoll(Poll poll)
        {
            var polls = GetPolls().ToList();
            poll.Id = Guid.NewGuid();
            polls.Add(poll);
            string json = JsonConvert.SerializeObject(polls);
            System.IO.File.WriteAllText(_fileName, json);
        }

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
