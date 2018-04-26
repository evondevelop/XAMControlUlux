using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMIO.Ulux.Ump.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class XAMUmpStateFlags
    {
        /// <summary>
        /// The not initialized
        /// </summary>
        public bool NotInitialized = true;
        /// <summary>
        /// The last state received
        /// </summary>
        public DateTime LastStateReceived;

        /// <summary>
        /// Dieses Bit gibt an, ob der Lichtsensor das Umgebungslicht als
        /// hell oder dunkel einstuft. Über das Bit 0 in den ControlFlags
        /// kann eingestellt werden, ob bei Änderungen des Lichtsensors
        /// ein Paket versendet werden soll.
        /// </summary>
        public bool LightSensor;
        /// <summary>
        /// Dieses Bit gibt an, ob der Näherungssensor ein Objekt
        /// innerhalb seiner Reichweite (ca. 25cm) erkannt hat. Diese
        /// Funktion kann z.B. als Bewegungsmelder verwendet werden
        /// um damit das Licht im Gang einzuschalten. Über das Bit 1 in
        /// den ControlFlags kann eingestellt werden, ob bei Änderungen
        /// des Näherungssensor ein Paket versendet werden soll.
        /// </summary>
        public bool ProximitySensor;
        /// <summary>
        /// Dieses Bit gibt an, ob das Display und die Displaybeleuchtung
        /// eingeschaltet sind. Über das Bit 2 in den ControlFlags kann
        /// eingestellt werden, ob bei Änderungen der Displaybeleuchtung
        /// ein Paket versendet werden soll.
        /// </summary>
        public bool DisplayActive;
        /// <summary>
        /// Dieses Bit gibt an, ob eine Audio Funktion aktiv ist. Es ist dabei
        /// egal, ob es sich um eine Wiedergabe (Play) oder Aufnahme
        /// (Record) handelt! Über das Bit 3 in den ControlFlags kann
        /// eingestellt werden, ob bei Änderungen des Audiostatus ein
        /// Paket versendet werden soll.
        /// </summary>
        public bool AudioActive;
        /// <summary>
        /// Dieses Bit gibt an, ob die Intro-Animation aktiv ist.
        /// </summary>
        public bool IntroActive;

        /// <summary>
        /// Wenn dieses Bit gesetzt ist, so benötigt der Teilnehmer ein
        /// TimeSync Paket. Sobald der Teilnehmer ein TimeSync Paket
        /// erhalten hat, wir dieses Bit gelöscht. Das Bit kann jedoch zu
        /// beliebigen anderen Zeitpunkten wieder gesetzt werden!
        /// </summary>
        public bool TimeRequest;
        /// <summary>
        /// Wenn dieses Bit gesetzt ist, so benötigt der Teilnehmer ein
        /// Paket mit dem aktuellen ControlBlock. Sobald der Teilnehmer
        /// ein Paket mit einem ControlBlock erhalten hat, wird dieses Bit
        /// gelöscht. Diese Funktion dient Primär dazu, damit die
        /// Steuerung erfährt, dass der Teilnehmer (z.B. nach einer
        /// Softwareaktualisierung) neu gestartet hat.
        /// </summary>
        public bool InitRequest;
        /// <summary>
        /// Wenn dieses Bit den Wert 1 hat, so hat der Teilnehmer einen
        /// schwerwiegenden Fehler und muss überprüft werden.
        /// </summary>
        public bool InternalError;


        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpStateFlags"/> class.
        /// </summary>
        public XAMUmpStateFlags()
        {
            NotInitialized = true;
            LastStateReceived = DateTime.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XAMUmpStateFlags"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public XAMUmpStateFlags(byte[] data)
        {
            NotInitialized = false;
            LightSensor = ((data[0] & 0x01) > 0) ? true : false;
            ProximitySensor = ((data[0] & 0x02) > 0) ? true : false;
            DisplayActive = ((data[0] & 0x04) > 0) ? true : false;
            AudioActive = ((data[0] & 0x08) > 0) ? true : false;
            IntroActive = ((data[0] & 0x10) > 0) ? true : false;
            TimeRequest = ((data[0] & 0x20) > 0) ? true : false;
            InitRequest = ((data[0] & 0x40) > 0) ? true : false;
            InternalError = ((data[0] & 0x80) > 0) ? true : false;
            LastStateReceived = DateTime.Now;
        }
    }
}
