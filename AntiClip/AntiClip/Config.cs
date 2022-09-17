using Exiled.API.Interfaces;

namespace AntiClip
{
    public class Config:IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public string Message { get; set; } = "Alerte ! tu te trouve hors de la map !!! Réinitialisation dans ";

        public int SizePercent { get; set; } = 200;
    }
}