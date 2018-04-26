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
    public class XAMUmpStreamFlags
    {
        /// <summary>
        /// The Acknowledge
        /// Ist dieses Bit aktiviert, so schickt der Teilnehmer ein
        /// Acknowledgepaket an den Absender retour, sobald die Daten
        /// erfolgreich verarbeitet wurden und das nächste Paket
        /// geschickt werden kann. Das Acknowledgepaket besteht
        /// lediglich aus dem Deskriptor (so wie es an den u::Lux Switch
        /// gesendet wurde) und ist daher nur 16 Bytes groß.
        /// </summary>
        public bool Acknowledge;

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] Bytes
        {
            get
            {
                byte[] b = new byte[4];

                if (Acknowledge)
                    b[0] |= 0x01;

                return b;
            }
        }
    }
}
