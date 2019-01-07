using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitApp
{
    class CustomerTest
    {
        public CustomerTest(int num, string no, string co)
        {
            id = num;
            nome = no;
            cognome = co;
            DatiPersonali = new DetailTest
            {
                Azienda = "Sigla",
                DataNascita = new DateTime(1989, 01, 16),
                Stipendio = 1200.98m
            };
            
        }

        public int id { get; set; }
        public string nome { get; set; }
        public string cognome { get; set; }
        public DetailTest DatiPersonali { get; set; }
    }
}
