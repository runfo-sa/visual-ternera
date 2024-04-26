namespace Main.Model
{
    public enum Status : byte
    {
        Okay = 0,
        Desactualizada = 1,
        Sobrantes = 2,
        DesactualizadaSobrantes = 3,
        MultipleInstalaciones = 4,
        NoInstalado = 5,
    }
}