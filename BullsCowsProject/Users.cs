using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO; 
namespace BullsCowsProject
{
    public class Users
    {
         
        public DateTime Datetime        { set; get; }
        public string Name  { set; get; }
        public int Move {get;set;}
        public override string ToString()
        {
            return Move + " " + Name + " " + Datetime;
        }
    }
    public class ScoreBoard
    {
        public static List<Users> UsersScore{get;set;}
        public static void LoadScoreBoard()
        {
            try
            {
                using (StreamReader r = new StreamReader("user.json"))
                {

                    string json = r.ReadToEnd();
                    UsersScore = JsonConvert.DeserializeObject<List<Users>>(json);
                }
                var Sorted = (from u in UsersScore orderby u.Move select u).ToList<Users>();
                UsersScore = Sorted;
            }
            catch(Exception)
            {
                UsersScore = new List<Users>();
            }
        }
        public static void SaveScoreBoard()
        {
            string json = JsonConvert.SerializeObject(UsersScore);
            File.WriteAllText(@"user.json", json);

        }
        public static void Add(Users user)
        {
            UsersScore.Add(user);
           var Sorted = (from u in UsersScore orderby u.Move select u).ToList<Users>();
           UsersScore = Sorted;
        }
        
    }
}
