using System.Collections.Generic;
using Godot;

namespace KingdomCome.Scripts.Other;

public static class NameGenerator
{
    public static List<string> FirstNames =
    [
        "Aethelred", "Astrid", "Bjorn", "Beatrice", "Cnut", "Cecily", "Dagfinn", "Edith", "Eirik", "Eleanor", "Finn",
        "Elfrida", "Godwin", "Estrid", "Hakon", "Freydis", "Ivar", "Gytha", "Jarl", "Hilda", "Knut", "Ingrid", "Leif",
        "Judith", "Magnus", "Matilda", "Olaf", "Sigrid", "Ragnar", "Tove", "Sigurd", "Aelgifu", "Thorfinn", "Bridget",
        "Ulf", "Eadgifu", "Valdemar", "Godiva", "Wulfstan", "Helga", "Alfred", "Idunn", "Baldric", "Kara", "Cedric",
        "Leofrun", "Dreng", "Mildrith", "Ealdred", "Ragnhild", "Faramund", "Solveig", "Gunnar", "Wynflaed", "Harald",
        "Ysabel", "Ingolf", "Aethelflaed", "Jorund", "Blanche", "Ketil", "Cwenhild", "Loki", "Drifa", "Mundric",
        "Elswyth", "Njal", "Gerd", "Orm", "Herleva", "Peder", "Iseult", "Rollo", "Katla", "Sigfrid", "Lagertha", "Thor",
        "Morrigan", "Ubbe", "Rowena", "Vidar", "Seaxburh", "Wiglaf", "Tyr", "Aethelbald", "Wulfhild", "Beowulf", "Aud",
        "Ceolwulf", "Brynhild", "Dag", "Elinor", "Ethelbert", "Gwenllian", "Fridleif", "Hawise", "Gorm", "Ingeborg",
        "Harwin", "Leofwen", "Ingmar", "Merewen", "Jorvik", "Runa", "Kjartan", "Swanhild"
    ];

    public static List<string> LastNames =
    [
        "Athelstan", "Baldwin", "Beaumont", "Bjornsson", "Blake", "Brandon", "Canmore", "Carlson", "Carter", "Cnutsson",
        "Corbin", "Dagson", "Dane", "De Clare", "De Lacy", "Drake", "Dunbar", "Eiriksson", "Fitzroy", "Gainsborough",
        "Godwinson", "Grimsson", "Gunnarson", "Halvorsen", "Hardrada", "Haroldson", "Hawke", "Hemming", "Hrothgarson",
        "Huntington", "Ingram", "Jarlson", "Knudsen", "Langton", "Leifsson", "Lovel", "Loxley", "Magnusson",
        "Mandeville", "Montgomery", "Morrison", "Northman", "Oswinson", "Overton", "Ragnarrson", "Ravenwood", "Rollo",
        "Roslin", "Sigurdsson", "Stafford", "Stanley", "Stark", "Strang", "Sweynsson", "Tate", "Thorson",
        "Thorvaldsson", "Tostig", "Ulfsson", "Viking", "Walters", "Ward", "Whitlock", "Wiglafson", "Williamson",
        "Windsor", "Winter", "Wood", "Wulfstan", "Yngvarsson", "York", "Alfricsson", "Arkwright", "Aylward", "Barlow",
        "Baxter", "Blakeley", "Braxton", "Briar", "Broadbent", "Bryce", "Carrow", "Chamberlain", "Chetwynd", "Colville",
        "Cromwell", "Davenport", "de Bohun", "de Warenne", "Eldridge", "Everton", "Fairfax", "Faulkner", "Gawain",
        "Greystone", "Harkness", "Hart", "Hemmingway", "Langford", "Osricson", "Redmayne", "Ridgewell", "Thorburn",
        "Whitelock", "Yarborough"
    ];

    private static RandomNumberGenerator _numberGenerator = new();

    public static string GenerateFirstName()
    {
       return FirstNames[_numberGenerator.RandiRange(0,FirstNames.Count-1)];
    }
    public static string GenerateLastName()
    {
        return  LastNames[_numberGenerator.RandiRange(0,LastNames.Count-1)];
    }
}