﻿using System;
using System.Collections.Generic;
using System.Text;

namespace quarzApp
{
    class Cliente
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string cognome { get; set; }
        public Dettaglio DatiPersonali { get; set; }
    }
}
