﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Agent.AgentTasks
{
    public class WhoisServerResolver
    {
        //;WHOIS Servers List
        //;Maintained by Nir Sofer
        //;This servers list if freely available for any use and without any restriction. 
        //;For more information: http://www.nirsoft.net/whois_servers_list.html
        //;Last updated on 09/03/2014
        private static Dictionary<string, string> WhoisServerCollection = new Dictionary<string, string>()
        {
            {"ac", "whois.nic.ac"},
            {"ae", "whois.aeda.net.ae"},
            {"aero", "whois.aero"},
            {"af", "whois.nic.af"},
            {"ag", "whois.nic.ag"},
            {"al", "whois.ripe.net"},
            {"am", "whois.amnic.net"},
            {"as", "whois.nic.as"},
            {"asia", "whois.nic.asia"},
            {"at", "whois.nic.at"},
            {"au", "whois.aunic.net"},
            {"ax", "whois.ax"},
            {"az", "whois.ripe.net"},
            {"ba", "whois.ripe.net"},
            {"be", "whois.dns.be"},
            {"bg", "whois.register.bg"},
            {"bi", "whois.nic.bi"},
            {"biz", "whois.neulevel.biz"},
            {"bj", "www.nic.bj"},
            {"br", "whois.nic.br"},
            {"br.com", "whois.centralnic.com"},
            {"bt", "whois.netnames.net"},
            {"by", "whois.cctld.by"},
            {"bz", "whois.belizenic.bz"},
            {"ca", "whois.cira.ca"},
            {"cat", "whois.cat"},
            {"cc", "whois.nic.cc"},
            {"cd", "whois.nic.cd"},
            {"ch", "whois.nic.ch"},
            {"ck", "whois.nic.ck"},
            {"cl", "whois.nic.cl"},
            {"cn", "whois.cnnic.net.cn"},
            {"cn.com", "whois.centralnic.com"},
            {"co", "whois.nic.co"},
            {"co.nl", "whois.co.nl"},
            {"com", "whois.verisign-grs.com"},
            {"coop", "whois.nic.coop"},
            {"cx", "whois.nic.cx"},
            {"cy", "whois.ripe.net"},
            {"cz", "whois.nic.cz"},
            {"de", "whois.denic.de"},
            {"dk", "whois.dk-hostmaster.dk"},
            {"dm", "whois.nic.cx"},
            {"dz", "whois.nic.dz"},
            {"edu", "whois.educause.net"},
            {"ee", "whois.tld.ee"},
            {"eg", "whois.ripe.net"},
            {"es", "whois.nic.es"},
            {"eu", "whois.eu"},
            {"eu.com", "whois.centralnic.com"},
            {"fi", "whois.ficora.fi"},
            {"fo", "whois.nic.fo"},
            {"fr", "whois.nic.fr"},
            {"gb", "whois.ripe.net"},
            {"gb.com", "whois.centralnic.com"},
            {"gb.net", "whois.centralnic.com"},
            {"qc.com", "whois.centralnic.com"},
            {"ge", "whois.ripe.net"},
            {"gl", "whois.nic.gl"},
            {"gm", "whois.ripe.net"},
            {"gov", "whois.nic.gov"},
            {"gr", "whois.ripe.net"},
            {"gs", "whois.nic.gs"},
            {"hk", "whois.hknic.net.hk"},
            {"hm", "whois.registry.hm"},
            {"hn", "whois2.afilias-grs.net"},
            {"hr", "whois.dns.hr"},
            {"hu", "whois.nic.hu"},
            {"hu.com", "whois.centralnic.com"},
            {"id", "whois.pandi.or.id"},
            {"ie", "whois.domainregistry.ie"},
            {"il", "whois.isoc.org.il"},
            {"in", "whois.inregistry.net"},
            {"info", "whois.afilias.info"},
            {"int", "whois.isi.edu"},
            {"io", "whois.nic.io"},
            {"iq", "vrx.net"},
            {"ir", "whois.nic.ir"},
            {"is", "whois.isnic.is"},
            {"it", "whois.nic.it"},
            {"je", "whois.je"},
            {"jobs", "jobswhois.verisign-grs.com"},
            {"jp", "whois.jprs.jp"},
            {"ke", "whois.kenic.or.ke"},
            {"kg", "whois.domain.kg"},
            {"kz", "whois.nic.kz"},
            {"kr", "whois.nic.or.kr"},
            {"la", "whois2.afilias-grs.net"},
            {"li", "whois.nic.li"},
            {"lt", "whois.domreg.lt"},
            {"lu", "whois.restena.lu"},
            {"lv", "whois.nic.lv"},
            {"ly", "whois.lydomains.com"},
            {"ma", "whois.iam.net.ma"},
            {"mc", "whois.ripe.net"},
            {"md", "whois.nic.md"},
            {"me", "whois.nic.me"},
            {"mil", "whois.nic.mil"},
            {"mk", "whois.ripe.net"},
            {"mobi", "whois.dotmobiregistry.net"},
            {"ms", "whois.nic.ms"},
            {"mt", "whois.ripe.net"},
            {"mu", "whois.nic.mu"},
            {"mx", "whois.nic.mx"},
            {"my", "whois.mynic.net.my"},
            {"name", "whois.nic.name"},
            {"net", "whois.verisign-grs.com"},
            {"nf", "whois.nic.cx"},
            {"ng", "whois.nic.net.ng"},
            {"nl", "whois.domain-registry.nl"},
            {"no", "whois.norid.no"},
            {"no.com", "whois.centralnic.com"},
            {"nu", "whois.nic.nu"},
            {"nz", "whois.srs.net.nz"},
            {"org", "whois.pir.org"},
            {"pl", "whois.dns.pl"},
            {"pr", "whois.nic.pr"},
            {"pro", "whois.registrypro.pro"},
            {"pt", "whois.dns.pt"},
            {"pw", "whois.nic.pw"},
            {"ro", "whois.rotld.ro"},
            {"ru", "whois.tcinet.ru"},
            {"sa", "saudinic.net.sa"},
            {"sa.com", "whois.centralnic.com"},
            {"sb", "whois.nic.net.sb"},
            {"sc", "whois2.afilias-grs.net"},
            {"se", "whois.nic-se.se"},
            {"se.com", "whois.centralnic.com"},
            {"se.net", "whois.centralnic.com"},
            {"sg", "whois.nic.net.sg"},
            {"sh", "whois.nic.sh"},
            {"si", "whois.arnes.si"},
            {"sk", "whois.sk-nic.sk"},
            {"sm", "whois.nic.sm"},
            {"st", "whois.nic.st"},
            {"so", "whois.nic.so"},
            {"su", "whois.tcinet.ru"},
            {"tc", "whois.adamsnames.tc"},
            {"tel", "whois.nic.tel"},
            {"tf", "whois.nic.tf"},
            {"th", "whois.thnic.net"},
            {"tj", "whois.nic.tj"},
            {"tk", "whois.nic.tk"},
            {"tl", "whois.domains.tl"},
            {"tm", "whois.nic.tm"},
            {"tn", "whois.ati.tn"},
            {"to", "whois.tonic.to"},
            {"tp", "whois.domains.tl"},
            {"tr", "whois.nic.tr"},
            {"travel", "whois.nic.travel"},
            {"tw", "whois.twnic.net.tw"},
            {"tv", "whois.nic.tv"},
            {"tz", "whois.tznic.or.tz"},
            {"ua", "whois.ua"},
            {"uk", "whois.nic.uk"},
            {"uk.com", "whois.centralnic.com"},
            {"uk.net", "whois.centralnic.com"},
            {"ac.uk", "whois.ja.net"},
            {"gov.uk", "whois.ja.net"},
            {"us", "whois.nic.us"},
            {"us.com", "whois.centralnic.com"},
            {"uy", "nic.uy"},
            {"uy.com", "whois.centralnic.com"},
            {"uz", "whois.cctld.uz"},
            {"va", "whois.ripe.net"},
            {"vc", "whois2.afilias-grs.net"},
            {"ve", "whois.nic.ve"},
            {"vg", "whois.adamsnames.tc"},
            {"ws", "whois.website.ws"},
            {"xxx", "whois.nic.xxx"},
            {"yu", "whois.ripe.net"},
            {"za.com", "whois.centralnic.com"},
            {"рф", "whois.tcinet.ru"},
            {"москва", "whois.nic.ru"}
        };

        public static string GetWhoisServerName(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
            {
                throw new ArgumentNullException("domainName", "domainName value cannot be a null, empty or whitespace");
            }

            if (domainName.Contains('.') == false)
            {
                throw new ArgumentException("Not a valid domain name: " + domainName, "domainName");
            }

            var tld = GetTLDFromDomainName(domainName);
            if (string.IsNullOrWhiteSpace(tld))
            {
                throw new ArgumentException("Not a valid domain name: " + domainName, "domainName");
            }

            tld = tld.Trim();
            if (WhoisServerCollection.ContainsKey(tld))
            {
                return WhoisServerCollection[tld];
            }

            return null;
        }

        public static string GetTLDFromDomainName(string domainName)
        {
            domainName = domainName.ToLower();
            var sortedTldList = GetAvailableTLDNameList().OrderByDescending(t => t.Length).ThenBy(t => t).ToList();
            for (var i = 0; i < sortedTldList.Count; i++)
            {
                if (domainName.EndsWith("." + sortedTldList[i]))
                    return sortedTldList[i];
            }

            return null;
        }

        public static IEnumerable<string> GetAvailableTLDNameList()
        {
            return WhoisServerCollection.Keys;
        }
    }
}
