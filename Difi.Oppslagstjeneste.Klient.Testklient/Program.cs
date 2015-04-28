﻿using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using ApiClientShared;
using ApiClientShared.Enums;

namespace Difi.Oppslagstjeneste.Klient.Testklient
{
    class Program
    {
        static void Main(string[] args)
        {
            var konfig = new OppslagstjenesteKonfigurasjon
            {
                ServiceUri = new Uri("https://kontaktinfo-ws-ver2.difi.no/kontaktinfo-external/ws-v4")
            };
            
            Logging.Initialize(konfig);
            Logging.Log(TraceEventType.Information, "> Starter program!");

            var storeMy = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            storeMy.Open(OpenFlags.ReadOnly);

            var avsenderSertifikat = CertificateUtility.SenderCertificate("B0CB922214D11E8CE993838DB4C6D04C0C0970B8", Language.Norwegian);
            var valideringssertifikat =
                CertificateUtility.ReceiverCertificate("a4 7d 57 ea f6 9b 62 77 10 fa 0d 06 ec 77 50 0b af 71 c4 32",
                    Language.Norwegian);

            ResourceUtility rr = new ResourceUtility("Difi.Oppslagstjeneste.Klient.Testklient.Resources");
            var certBytes = rr.ReadAllBytes(true, "cert.idporten-ver2.difi.no-v2.crt");
            var valideringsSertifikat = new X509Certificate2(certBytes);
            
            OppslagstjenesteKlient register = new OppslagstjenesteKlient(avsenderSertifikat, valideringsSertifikat, konfig);

            var endringer = register.HentEndringer(886730, 
                Informasjonsbehov.Kontaktinfo | 
                Informasjonsbehov.Sertifikat | 
                Informasjonsbehov.SikkerDigitalPost |
                Informasjonsbehov.Person);
            
            var personer = register.HentPersoner(new string[] { "08077000292" }, 
                Informasjonsbehov.Sertifikat | 
                Informasjonsbehov.Kontaktinfo | 
                Informasjonsbehov.SikkerDigitalPost);
           
            var printSertifikat = register.HentPrintSertifikat();
            Console.ReadKey();
        }
    }
}
