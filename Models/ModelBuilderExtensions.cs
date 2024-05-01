using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            ////SEED SURVEY QUESTION CATEGORY
            //modelBuilder.Entity<SurveyQuestionsCategory>().HasData(new SurveyQuestionsCategory
            //{
            //    Id = 1,
            //    Name = "Appearance"
            //},
            //new SurveyQuestionsCategory
            //{
            //    Id = 2,
            //    Name = "Personality"
            //},
            //new SurveyQuestionsCategory
            //{
            //    Id = 3,
            //    Name = "Habits"
            //}

            //);
            ////SEED SURVEY QUESTION CATEGORY

            ////SEED SURVEY QUESTIONs
            //modelBuilder.Entity<SurveyQuestions>().HasData(new SurveyQuestions
            //{
            //    Id = 1,
            //    Category = new SurveyQuestionsCategory(),
            //    Name = "Appearance"
            //},
            //new SurveyQuestions
            //{
            //    Id = 2,
            //    Name = "Personality"
            //},
            //new SurveyQuestions
            //{
            //    Id = 3,
            //    Name = "Habits"
            //}

            //);
            ////SEED SURVEY QUESTIONs
        }

    }
}
