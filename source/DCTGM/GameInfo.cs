using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DCTGM
{
    public enum GameBrand
    {
        Steam, Origin, Blizzard, Ubisoft, Garena
    }

    internal class GameInfo
    {
        private List<GameBrand> gameBrands;

        public GameInfo()
        {
            gameBrands = new List<GameBrand>();

            if (GameInfoCollector.CheckSteam())
                gameBrands.Add(GameBrand.Steam);
            if (GameInfoCollector.CheckOrigin())
                gameBrands.Add(GameBrand.Origin);
            if (GameInfoCollector.CheckBlizzard())
                gameBrands.Add(GameBrand.Blizzard);
            if (GameInfoCollector.CheckUbisoft())
                gameBrands.Add(GameBrand.Ubisoft);
            if (GameInfoCollector.CheckGarena())
                gameBrands.Add(GameBrand.Garena);
        }

        public JObject GetInformation()
        {
            JObject jObject = new JObject();
            JArray jaGames = new JArray();

            gameBrands.ForEach(r => jaGames.Add(r.ToString()));
            JProperty jGames = new JProperty("games", jaGames);

            jObject.Add(jGames);

            return jObject;
        }
    }
}