using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SteamTotalHours
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your custom steamUrl:");
            string steamUrl = Console.ReadLine();
            XElement xElement = getXmlResponse(steamUrl);

            List<XElement> allGames = (from item in xElement.Descendants("games").Descendants("game") select item).ToList<XElement>();
            var playedGames = GetPlayedGames(allGames, true);


            Console.WriteLine("total games: " + allGames.Count);
            Console.WriteLine("played games: " + playedGames.Count);


            foreach (var game in playedGames)
            {
                Console.WriteLine("* " + game.Key + " Hours: " + game.Value);
            }


            //get total play time
            Console.WriteLine("==============================================================");
            Console.WriteLine("Total play time in hours: {0}", getTotalPlayTime(playedGames));
            Console.WriteLine("==============================================================");
            Console.ReadLine();
        }

        private static XElement getXmlResponse(string steamUrl)
        {
            var url = "https://steamcommunity.com/id/" + steamUrl + "/games?tab=all&xml=1";
            XmlTextReader reader = new XmlTextReader(url);
            XmlDocument responseXML = new XmlDocument();
            responseXML.Load(reader);

            XElement xElement = XElement.Load(url);
            return xElement;
        }

        private static List<KeyValuePair<string, float>> GetPlayedGames(List<XElement> allGames, bool sortByHoursDescending)
        {
            var playedGames = new Dictionary<string, float>();


            foreach (var game in allGames)
            {
                if (game.Element("hoursOnRecord") != null)
                {
                    playedGames.Add(game.Element("name").Value, float.Parse(game.Element("hoursOnRecord").Value));
                }
            }

            List<KeyValuePair<string, float>> playedGamesKVP = playedGames.ToList();

            if (sortByHoursDescending)
            {
                playedGamesKVP.Sort((x, y) => y.Value.CompareTo(x.Value));
            }

            return playedGamesKVP;
        }


        private static float getTotalPlayTime(List<KeyValuePair<string, float>> playedGames)
        {
            float totalTime = 0;

            foreach (var game in playedGames) 
            {
                totalTime += game.Value;
            }
            return totalTime;
        }
    }
}
