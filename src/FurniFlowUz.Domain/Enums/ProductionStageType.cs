namespace FurniFlowUz.Domain.Enums;

public enum ProductionStageType
{
    Sawing,       // Kesish ishlari: Razmer, Arra
    Routing,      // Frezerlash: Shipon, Pres, Rover
    EdgeBanding,  // Qirralarni yopish: Kromka
    Sanding,      // Silliqlash: Shkurka
    Assembly,     // Yig'ish: OTK, Qadoqlash
    Finishing,    // Bo'yash: Pardozchi, Grunt, Kraska, Qurutish
    Painting,     // Bo'yash jarayonlari
    QualityControl // Sifat nazorati
}
