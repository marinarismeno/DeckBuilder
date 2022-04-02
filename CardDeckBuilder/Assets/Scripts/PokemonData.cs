using System.Collections.Generic;

[System.Serializable]
public class Ability
{
    public string name;
    public string text;
    public string type;
}

[System.Serializable]
public class Attack
{
    public string name;
    public List<string> cost;
    public int convertedEnergyCost;
    public string damage;
    public string text;
}

[System.Serializable]
public class Weakness
{
    public string type;
    public string value;
}

[System.Serializable]
public class Resistance
{
    public string type;
    public string value;
}

[System.Serializable]
public class Legalities
{
    public string unlimited;
    public string expanded;
    public string standard;
}

[System.Serializable]
public class Images
{
    public string symbol;
    public string logo;
    public string small;
    public string large;
}

[System.Serializable]
public class Set
{
    public string id;
    public string name;
    public string series;
    public int printedTotal;
    public int total;
    public Legalities legalities;
    public string ptcgoCode;
    public string releaseDate;
    public string updatedAt;
    public Images images;
}

[System.Serializable]
public class ReverseHolofoil
{
    public double low;
    public double mid;
    public double high;
    public double market;
    public double? directLow;
}

[System.Serializable]
public class Holofoil
{
    public double low;
    public double mid;
    public double high;
    public double market;
    public double directLow;
}

[System.Serializable]
public class Normal
{
    public double low;
    public double mid;
    public double high;
    public double market;
    public double directLow;
}

[System.Serializable]
public class UnlimitedHolofoil
{
    public double low;
    public double mid;
    public double high;
    public double market;
    public double? directLow;
}

[System.Serializable]
public class _1stEditionHolofoil
{
    public double low;
    public double mid;
    public double high;
    public double market;
    public double? directLow;
}

[System.Serializable]
public class Prices
{
    public ReverseHolofoil reverseHolofoil;
    public Holofoil holofoil;
    public Normal normal;
    public UnlimitedHolofoil unlimitedHolofoil;
    public _1stEditionHolofoil _1stEditionHolofoil;
    public double averageSellPrice;
    public double lowPrice;
    public double trendPrice;
    public double reverseHoloSell;
    public double reverseHoloLow;
    public double reverseHoloTrend;
    public double lowPriceExPlus;
    public double avg1;
    public double avg7;
    public double avg30;
    public double reverseHoloAvg1;
    public double reverseHoloAvg7;
    public double reverseHoloAvg30;
}

[System.Serializable]
public class Tcgplayer
{
    public string url;
    public string updatedAt;
    public Prices prices;
}

[System.Serializable]
public class Cardmarket
{
    public string url;
    public string updatedAt;
    public Prices prices;
}

[System.Serializable]
public class Data
{
    public string id;
    public string name;
    public string supertype;
    public List<string> subtypes;
    public string level;
    public string hp;
    public List<string> types;
    public string evolvesFrom;
    public List<Ability> abilities;
    public List<Attack> attacks;
    public List<Weakness> weaknesses;
    public List<Resistance> resistances;
    public List<string> retreatCost;
    public int convertedRetreatCost;
    public Set set;
    public string number;
    public string artist;
    public string rarity = "";
    public int rarityNo;
    public List<int> nationalPokedexNumbers;
    public Legalities legalities;
    public Images images;
    public Tcgplayer tcgplayer;
    public Cardmarket cardmarket;
    public List<string> evolvesTo;
    public string flavorText;
    public List<string> rules;
    public string regulationMark;
}

[System.Serializable]
public class PokemonMultiData
{
    public List<Data> data;
    public int page;
    public int pageSize;
    public int count;
    public int totalCount;
}

