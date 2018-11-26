using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ProStudCreator
{
    public static class StringExtensions
    {
        private static readonly string[] TLDs =
        {
            "ABB", "ABBOTT", "ABOGADO", "AC", "ACADEMY", "ACCENTURE", "ACCOUNTANT", "ACCOUNTANTS", "ACTIVE", "ACTOR",
            "AD", "ADS", "ADULT", "AE", "AERO", "AF", "AFL", "AG", "AGENCY", "AI", "AIG", "AIRFORCE", "AL", "ALLFINANZ",
            "ALSACE", "AM", "AMSTERDAM", "AN", "ANDROID", "AO", "APARTMENTS", "AQ", "AQUARELLE", "AR", "ARCHI", "ARMY",
            "ARPA", "AS", "ASIA", "ASSOCIATES", "AT", "ATTORNEY", "AU", "AUCTION", "AUDIO", "AUTO", "AUTOS", "AW", "AX",
            "AXA", "AZ", "BA", "BAND", "BANK", "BAR", "BARCLAYCARD", "BARCLAYS", "BARGAINS", "BAUHAUS", "BAYERN", "BB",
            "BBC", "BD", "BE", "BEER", "BERLIN", "BEST", "BF", "BG", "BH", "BI", "BID", "BIKE", "BINGO", "BIO", "BIZ",
            "BJ", "BLACK", "BLACKFRIDAY", "BLOOMBERG", "BLUE", "BM", "BMW", "BN", "BNPPARIBAS", "BO", "BOATS", "BOND",
            "BOO", "BOUTIQUE", "BR", "BRIDGESTONE", "BROKER", "BROTHER", "BRUSSELS", "BS", "BT", "BUDAPEST", "BUILD",
            "BUILDERS", "BUSINESS", "BUZZ", "BV", "BW", "BY", "BZ", "BZH", "CA", "CAB", "CAFE", "CAL", "CAMERA", "CAMP",
            "CANCERRESEARCH", "CANON", "CAPETOWN", "CAPITAL", "CARAVAN", "CARDS", "CARE", "CAREER", "CAREERS", "CARS",
            "CARTIER", "CASA", "CASH", "CASINO", "CAT", "CATERING", "CBN", "CC", "CD", "CENTER", "CEO", "CERN", "CF",
            "CFA", "CFD", "CG", "CH", "CHANNEL", "CHAT", "CHEAP", "CHLOE", "CHRISTMAS", "CHROME", "CHURCH", "CI",
            "CISCO", "CITIC", "CITY", "CK", "CL", "CLAIMS", "CLEANING", "CLICK", "CLINIC", "CLOTHING", "CLUB", "CM",
            "CN", "CO", "COACH", "CODES", "COFFEE", "COLLEGE", "COLOGNE", "COM", "COMMUNITY", "COMPANY", "COMPUTER",
            "CONDOS", "CONSTRUCTION", "CONSULTING", "CONTRACTORS", "COOKING", "COOL", "COOP", "CORSICA", "COUNTRY",
            "COUPONS", "COURSES", "CR", "CREDIT", "CREDITCARD", "CRICKET", "CRS", "CRUISES", "CU", "CUISINELLA", "CV",
            "CW", "CX", "CY", "CYMRU", "CYOU", "CZ", "DABUR", "DAD", "DANCE", "DATE", "DATING", "DATSUN", "DAY", "DCLK",
            "DE", "DEALS", "DEGREE", "DELIVERY", "DEMOCRAT", "DENTAL", "DENTIST", "DESI", "DESIGN", "DEV", "DIAMONDS",
            "DIET", "DIGITAL", "DIRECT", "DIRECTORY", "DISCOUNT", "DJ", "DK", "DM", "DNP", "DO", "DOCS", "DOG", "DOHA",
            "DOMAINS", "DOOSAN", "DOWNLOAD", "DURBAN", "DVAG", "DZ", "EARTH", "EAT", "EC", "EDU", "EDUCATION", "EE",
            "EG", "EMAIL", "EMERCK", "ENERGY", "ENGINEER", "ENGINEERING", "ENTERPRISES", "EPSON", "EQUIPMENT", "ER",
            "ERNI", "ES", "ESQ", "ESTATE", "ET", "EU", "EUROVISION", "EUS", "EVENTS", "EVERBANK", "EXCHANGE", "EXPERT",
            "EXPOSED", "EXPRESS", "FAIL", "FAITH", "FAN", "FANS", "FARM", "FASHION", "FEEDBACK", "FI", "FILM",
            "FINANCE", "FINANCIAL", "FIRMDALE", "FISH", "FISHING", "FIT", "FITNESS", "FJ", "FK", "FLIGHTS", "FLORIST",
            "FLOWERS", "FLSMIDTH", "FLY", "FM", "FO", "FOO", "FOOTBALL", "FOREX", "FORSALE", "FOUNDATION", "FR", "FRL",
            "FROGANS", "FUND", "FURNITURE", "FUTBOL", "GA", "GAL", "GALLERY", "GARDEN", "GB", "GBIZ", "GD", "GDN", "GE",
            "GENT", "GF", "GG", "GGEE", "GH", "GI", "GIFT", "GIFTS", "GIVES", "GL", "GLASS", "GLE", "GLOBAL", "GLOBO",
            "GM", "GMAIL", "GMO", "GMX", "GN", "GOLD", "GOLDPOINT", "GOLF", "GOO", "GOOG", "GOOGLE", "GOP", "GOV", "GP",
            "GQ", "GR", "GRAPHICS", "GRATIS", "GREEN", "GRIPE", "GS", "GT", "GU", "GUGE", "GUIDE", "GUITARS", "GURU",
            "GW", "GY", "HAMBURG", "HANGOUT", "HAUS", "HEALTHCARE", "HELP", "HERE", "HERMES", "HIPHOP", "HITACHI",
            "HIV", "HK", "HM", "HN", "HOCKEY", "HOLDINGS", "HOLIDAY", "HOMES", "HONDA", "HORSE", "HOST", "HOSTING",
            "HOUSE", "HOW", "HR", "HT", "HU", "IBM", "ICBC", "ICU", "ID", "IE", "IFM", "IL", "IM", "IMMO", "IMMOBILIEN",
            "IN", "INDUSTRIES", "INFINITI", "INFO", "ING", "INK", "INSTITUTE", "INSURE", "INT", "INTERNATIONAL",
            "INVESTMENTS", "IO", "IQ", "IR", "IRISH", "IS", "IT", "IWC", "JAVA", "JCB", "JE", "JETZT", "JEWELRY", "JM",
            "JO", "JOBS", "JOBURG", "JP", "JUEGOS", "KAUFEN", "KDDI", "KE", "KG", "KH", "KI", "KIM", "KITCHEN", "KIWI",
            "KM", "KN", "KOELN", "KOMATSU", "KP", "KR", "KRD", "KRED", "KW", "KY", "KYOTO", "KZ", "LA", "LACAIXA",
            "LAND", "LAT", "LATROBE", "LAWYER", "LB", "LC", "LDS", "LEASE", "LECLERC", "LEGAL", "LGBT", "LI", "LIAISON",
            "LIDL", "LIFE", "LIGHTING", "LIMITED", "LIMO", "LINK", "LK", "LOAN", "LOANS", "LOL", "LONDON", "LOTTE",
            "LOTTO", "LOVE", "LR", "LS", "LT", "LTDA", "LU", "LUPIN", "LUXE", "LUXURY", "LV", "LY", "MA", "MADRID",
            "MAIF", "MAISON", "MANAGEMENT", "MANGO", "MARKET", "MARKETING", "MARKETS", "MARRIOTT", "MC", "MD", "ME",
            "MEDIA", "MEET", "MELBOURNE", "MEME", "MEMORIAL", "MENU", "MG", "MH", "MIAMI", "MIL", "MINI", "MK", "ML",
            "MM", "MMA", "MN", "MO", "MOBI", "MODA", "MOE", "MONASH", "MONEY", "MORMON", "MORTGAGE", "MOSCOW",
            "MOTORCYCLES", "MOV", "MOVIE", "MP", "MQ", "MR", "MS", "MT", "MTN", "MTPC", "MU", "MUSEUM", "MV", "MW",
            "MX", "MY", "MZ", "NA", "NADEX", "NAGOYA", "NAME", "NAVY", "NC", "NE", "NEC", "NET", "NETWORK", "NEUSTAR",
            "NEW", "NEWS", "NEXUS", "NF", "NG", "NGO", "NHK", "NI", "NICO", "NINJA", "NISSAN", "NL", "NO", "NP", "NR",
            "NRA", "NRW", "NTT", "NU", "NYC", "NZ", "OKINAWA", "OM", "ONE", "ONG", "ONL", "ONLINE", "OOO", "ORACLE",
            "ORG", "ORGANIC", "OSAKA", "OTSUKA", "OVH", "PA", "PAGE", "PANERAI", "PARIS", "PARTNERS", "PARTS", "PARTY",
            "PE", "PF", "PG", "PH", "PHARMACY", "PHILIPS", "PHOTO", "PHOTOGRAPHY", "PHOTOS", "PHYSIO", "PIAGET", "PICS",
            "PICTET", "PICTURES", "PINK", "PIZZA", "PK", "PL", "PLACE", "PLUMBING", "PLUS", "PM", "PN", "POHL", "POKER",
            "PORN", "POST", "PR", "PRAXI", "PRESS", "PRO", "PROD", "PRODUCTIONS", "PROF", "PROPERTIES", "PROPERTY",
            "PS", "PT", "PUB", "PW", "PY", "QA", "QPON", "QUEBEC", "RACING", "RE", "REALTOR", "RECIPES", "RED",
            "REDSTONE", "REHAB", "REISE", "REISEN", "REIT", "REN", "RENT", "RENTALS", "REPAIR", "REPORT", "REPUBLICAN",
            "REST", "RESTAURANT", "REVIEW", "REVIEWS", "RICH", "RIO", "RIP", "RO", "ROCKS", "RODEO", "RS", "RSVP", "RU",
            "RUHR", "RUN", "RW", "RYUKYU", "SA", "SAARLAND", "SALE", "SAMSUNG", "SAP", "SARL", "SAXO", "SB", "SC",
            "SCA", "SCB", "SCHMIDT", "SCHOLARSHIPS", "SCHOOL", "SCHULE", "SCHWARZ", "SCIENCE", "SCOT", "SD", "SE",
            "SEAT", "SENER", "SERVICES", "SEW", "SEX", "SEXY", "SG", "SH", "SHIKSHA", "SHOES", "SHOW", "SHRIRAM", "SI",
            "SINGLES", "SITE", "SJ", "SK", "SKY", "SL", "SM", "SN", "SO", "SOCCER", "SOCIAL", "SOFTWARE", "SOHU",
            "SOLAR", "SOLUTIONS", "SONY", "SOY", "SPACE", "SPIEGEL", "SPREADBETTING", "SR", "ST", "STUDY", "STYLE",
            "SU", "SUCKS", "SUPPLIES", "SUPPLY", "SUPPORT", "SURF", "SURGERY", "SUZUKI", "SV", "SWISS", "SX", "SY",
            "SYDNEY", "SYSTEMS", "SZ", "TAIPEI", "TATAR", "TATTOO", "TAX", "TAXI", "TC", "TD", "TEAM", "TECH",
            "TECHNOLOGY", "TEL", "TEMASEK", "TENNIS", "TF", "TG", "TH", "THEATER", "TICKETS", "TIENDA", "TIPS", "TIRES",
            "TIROL", "TJ", "TK", "TL", "TM", "TN", "TO", "TODAY", "TOKYO", "TOOLS", "TOP", "TORAY", "TOSHIBA", "TOURS",
            "TOWN", "TOYS", "TR", "TRADE", "TRADING", "TRAINING", "TRAVEL", "TRUST", "TT", "TUI", "TV", "TW", "TZ",
            "UA", "UG", "UK", "UNIVERSITY", "UNO", "UOL", "US", "UY", "UZ", "VA", "VACATIONS", "VC", "VE", "VEGAS",
            "VENTURES", "VERSICHERUNG", "VET", "VG", "VI", "VIAJES", "VIDEO", "VILLAS", "VISION", "VLAANDEREN", "VN",
            "VODKA", "VOTE", "VOTING", "VOTO", "VOYAGE", "VU", "WALES", "WANG", "WATCH", "WEBCAM", "WEBSITE", "WED",
            "WEDDING", "WEIR", "WF", "WHOSWHO", "WIEN", "WIKI", "WILLIAMHILL", "WIN", "WME", "WORK", "WORKS", "WORLD",
            "WS", "WTC", "WTF", "XEROX", "XIN", "XN--1QQW23A", "XN--30RR7Y", "XN--3BST00M", "XN--3DS443G",
            "XN--3E0B707E", "XN--45BRJ9C", "XN--45Q11C", "XN--4GBRIM", "XN--55QW42G", "XN--55QX5D", "XN--6FRZ82G",
            "XN--6QQ986B3XL", "XN--80ADXHKS", "XN--80AO21A", "XN--80ASEHDB", "XN--80ASWG", "XN--90A3AC", "XN--90AIS",
            "XN--9ET52U", "XN--B4W605FERD", "XN--C1AVG", "XN--CG4BKI", "XN--CLCHC0EA0B2G2A9GCD", "XN--CZR694B",
            "XN--CZRS0T", "XN--CZRU2D", "XN--D1ACJ3B", "XN--D1ALF", "XN--ESTV75G", "XN--FIQ228C5HS", "XN--FIQ64B",
            "XN--FIQS8S", "XN--FIQZ9S", "XN--FJQ720A", "XN--FLW351E", "XN--FPCRJ9C3D", "XN--FZC2C9E2C", "XN--GECRJ9C",
            "XN--H2BRJ9C", "XN--HXT814E", "XN--I1B6B1A6A2E", "XN--IO0A7I", "XN--J1AMH", "XN--J6W193G",
            "XN--KCRX77D1X4A", "XN--KPRW13D", "XN--KPRY57D", "XN--KPUT3I", "XN--L1ACC", "XN--LGBBAT1AD8J",
            "XN--MGB9AWBF", "XN--MGBA3A4F16A", "XN--MGBAAM7A8H", "XN--MGBAB2BD", "XN--MGBAYH7GPA", "XN--MGBBH1A71E",
            "XN--MGBC0A9AZCG", "XN--MGBERP4A5D4AR", "XN--MGBPL2FH", "XN--MGBX4CD0AB", "XN--MXTQ1M", "XN--NGBC5AZD",
            "XN--NODE", "XN--NQV7F", "XN--NQV7FS00EMA", "XN--NYQY26A", "XN--O3CW4H", "XN--OGBPF8FL", "XN--P1ACF",
            "XN--P1AI", "XN--PGBS0DH", "XN--Q9JYB4C", "XN--QCKA1PMC", "XN--RHQV96G", "XN--S9BRJ9C", "XN--SES554G",
            "XN--UNUP4Y", "XN--VERMGENSBERATER - CTB", "XN--VERMGENSBERATUNG - PWB", "XN--VHQUV", "XN--VUQ861B",
            "XN--WGBH1C", "XN--WGBL6A", "XN--XHQ521B", "XN--XKC2AL3HYE2A", "XN--XKC2DL3A5EE0H", "XN--Y9A3AQ",
            "XN--YFRO4I67O", "XN--YGBI2AMMX", "XN--ZFR164B", "XXX", "XYZ", "YACHTS", "YANDEX", "YE", "YODOBASHI",
            "YOGA", "YOKOHAMA", "YOUTUBE", "YT", "ZA", "ZIP", "ZM", "ZONE", "ZUERICH", "ZW"
        };

        private static readonly Regex protocolExtender = new Regex("^[a-zA-Z0-9\\+]*://\\z");
        private static readonly Regex protocolMatcher = new Regex("^[a-zA-Z0-9\\+\\-]{2,}://\\z");

        private static readonly Regex emailMatcher = new Regex(
            "^[a-zA-Z0-9\\-_\\.]+@" + //password
            "([a-zA-Z0-9\\-]+\\.)+" + //host name
            "[a-zA-Z0-9\\-]{2,}\\z" //TLD
        );

        private static readonly Regex hostExtender = new Regex("^[\\p{L}0-9\\._\\-\\+@%\\:]*\\z");

        private static readonly Regex hostMatcher = new Regex(
            "^([a-zA-Z0-9%]+\\:[a-zA-Z0-9%]*@)?" + //password
            "([\\p{L}0-9\\-]+\\.)+" + //host name
            "[a-zA-Z0-9\\-]{2,}\\z" //TLD
        );

        private static readonly Regex portExtender = new Regex(@"^:[0-9]{0,5}\z");
        private static readonly Regex portMatcher = new Regex(@"^:[0-9]{1,5}\z");
        private static readonly Regex invalidTLDFollower = new Regex(@"^[\p{L}0-9\-]\z");
        private static readonly Regex docExtender = new Regex(@"^/[/\w#\?%\(\)\~\+\-_\.\,\=\&\;@]*\z");
        private static readonly Regex docShortener = new Regex(@"^[\.\,\!\;\:\?]\z");
        private static readonly Regex domainExtender = new Regex(@"^[A-z0-9\-\.]+\z");

        private static readonly Regex emptyLine = new Regex(@"^\s*\z");
        private static readonly Regex listUnordered = new Regex(@"^\s*([\*\-])"); // Examples: - List Item, * List Item

        private static readonly Regex listAlpha = new Regex(@"^\s*((?<index>[A-z])[\)\.])")
            ; // Examples: a) List Item, A. List Item

        private static readonly Regex listNumeric = new Regex(@"^\s*((?<index>\d+)[\)\.])")
            ; // Examples: 1) List Item, 2. List Item

        public static bool IsValidName(this string _text)
        {
            bool result;
            if (_text == null)
            {
                result = false;
            }
            else
            {
                var parts = _text.Trim().Split(' ');
                if (parts.Length < 2)
                {
                    result = false;
                }
                else
                {
                    var array = parts;
                    for (var i = 0; i < array.Length; i++)
                    {
                        var p = array[i];
                        if (p.Length < 2)
                        {
                            result = false;
                            return result;
                        }
                    }
                    result = true;
                }
            }
            return result;
        }

        public static bool IsValidEmail(this string _email)
        {
            if (_email == null)
                return false;

            var parts = _email.Trim().Split('@');
            if (parts.Length != 2)
                return false;

            if (parts[0].Length < 2)
                return false;

            parts = parts[1].Split('.');
            return parts.Length >= 2 && parts[0].Length >= 2 && parts[1].Length >= 2;
        }

        public static string FixupParagraph(this string _p)
        {
            _p = _p.Normalize(); // Convert to composite characters (e.g. a¨ -> ä). http://unicode.org/reports/tr15/

            _p = _p.Replace("\r\n", "\n").Replace('\r', '\n').Replace('“', '\"').Replace('”', '"').Replace('“', '\"')
                .Replace('«', '"').Replace('»', '"').Replace('¬', ' ')
                .Replace(' ', ' ').Replace(' ', ' ').Replace(' ', ' ').Replace(' ', ' ').Replace('—', '-')
                .Replace('–', '-').Replace("•", "-").Replace('’', '"').Replace('‘', '"');

            for (string oldP = null; oldP != _p;)
            {
                oldP = _p;
                _p = _p.Replace("  ", " ").Replace(" ,", ",").Replace("( ", "(").Replace(" )", ")").Replace(" /", "/")
                    .Replace("/ ", "/").Replace(" %", "%").Trim();
            }

            _p = Regex.Replace(_p, @"(\s*\n\s*){3,}",
                "\n\n"); // Limit to 2 consecutive newlines. Also include whitespace surrounding the empty lines.

            return _p;
        }

        public static IEnumerable<URLTuple> RecognizeURLs(this string _text)
        {
            var res = new List<URLTuple>(1);

            res.Add(new URLTuple(_text, null));
            if (!_text.Contains("."))
                return res;

            foreach (var tld in TLDs.OrderByDescending(s => s.Length))
                for (var i = 0; i < res.Count; i++)
                    if (res[i].URL == null)
                    {
                        var word = res[i].Text;
                        var upperWord = word.ToUpperInvariant();

                        var tldStart = upperWord.IndexOf("." + tld);
                        while (tldStart != -1)
                        {
                            var urlStart = tldStart;
                            var urlEnd = tldStart + tld.Length + 1;
                            var isValid = true;
                            var isEmail = false;

                            if (urlEnd < word.Length && invalidTLDFollower.IsMatch(word[urlEnd] + ""))
                                isValid = false;

                            //add host
                            if (isValid)
                            {
                                while (urlStart > 0 &&
                                       hostExtender.IsMatch(word.Substring(urlStart - 1, urlEnd - (urlStart - 1))))
                                    urlStart--;
                                if (!hostMatcher.IsMatch(word.Substring(urlStart, urlEnd - urlStart)))
                                    if (emailMatcher.IsMatch(word.Substring(urlStart, urlEnd - urlStart)))
                                        isEmail = true;
                                    else
                                        isValid = false;
                            }

                            //add protocol prefix to url if available
                            var hasProtocol = false;
                            if (isValid)
                                if (urlStart >= 4 && "://" == word.Substring(urlStart - 3, urlStart - (urlStart - 3)))
                                {
                                    var protocolStart = urlStart - 2;
                                    while (protocolStart > 0 &&
                                           protocolExtender.IsMatch(word.Substring(protocolStart - 1,
                                               urlStart - (protocolStart - 1))))
                                        protocolStart--;
                                    if (protocolMatcher.IsMatch(word.Substring(protocolStart, urlStart - protocolStart))
                                    )
                                    {
                                        urlStart = protocolStart;
                                        hasProtocol = true;
                                        if (isEmail)
                                            isValid = false;
                                    }
                                }

                            // Extend to end of domain name (if hyphenation happens inside hostname, e.g. top-level domain is cut off)
                            if (isValid)
                            {
                                var domainEnd = urlEnd;
                                while (domainEnd < word.Length &&
                                       domainExtender.IsMatch(word.Substring(urlEnd, domainEnd + 1 - urlEnd)))
                                    domainEnd++;

                                urlEnd = domainEnd;
                            }

                            //add port number
                            if (isValid)
                            {
                                var portEnd = urlEnd;
                                while (portEnd < word.Length &&
                                       portExtender.IsMatch(word.Substring(urlEnd, portEnd + 1 - urlEnd)))
                                    portEnd++;
                                if (portMatcher.IsMatch(word.Substring(urlEnd, portEnd - urlEnd)))
                                {
                                    if (urlEnd != portEnd && isEmail)
                                        isValid = false;
                                    urlEnd = portEnd;
                                }
                            }

                            //add document path
                            if (isValid && !isEmail)
                            {
                                var docEnd = urlEnd;
                                while (docEnd < word.Length &&
                                       docExtender.IsMatch(word.Substring(urlEnd, docEnd + 1 - urlEnd)))
                                    docEnd++;
                                if (docShortener.IsMatch(word[docEnd - 1] + ""))
                                    docEnd--;

                                var doc = word.Substring(urlEnd, docEnd - urlEnd);
                                var evenBrackets = (doc.Count(c => c == '(') - doc.Count(c => c == ')')) == 0;

                                if (urlStart > 0 && docEnd - 1 < word.Length && !evenBrackets)
                                {
                                    var lastChar = word[docEnd - 1];
                                    if (
                                        lastChar == ')' ||
                                        lastChar == ']' ||
                                        lastChar == '}'
                                        )
                                        docEnd--;
                                }

                                urlEnd = docEnd;
                            }

                            if (isValid)
                            {
                                var prefix = word.Substring(0, urlStart);
                                var url = word.Substring(urlStart, urlEnd - urlStart);
                                var postfix = word.Substring(urlEnd);

                                if (!hasProtocol && url.ToLowerInvariant().StartsWith("mailto:"))
                                    isValid = false;

                                if (isValid && !isEmail)
                                    try
                                    {
                                        new Uri(hasProtocol ? url : "http://" + url);
                                    }
                                    catch (UriFormatException)
                                    {
                                        isValid = false;
                                    }

                                if (isValid)
                                {
                                    res[i].Text = url;
                                    if (isEmail)
                                        res[i].URL = "mailto:" + url;
                                    else
                                        res[i].URL = hasProtocol ? url : "http://" + url;

                                    //append postfix
                                    if (postfix.Length > 0)
                                        if (i == res.Count - 1 || res[i + 1].URL != null)
                                            res.Insert(i + 1, new URLTuple(postfix, null));
                                        else
                                            res[i + 1].Text = postfix + res[i + 1].Text;

                                    //insert prefix
                                    if (prefix.Length > 0)
                                        if (i == 0 || res[i - 1].URL != null)
                                            res.Insert(i, new URLTuple(prefix, null));
                                        else
                                            res[i - 1].Text += prefix;

                                    //changed elements --> reevaluate it
                                    i--;
                                    break;
                                }
                            }

                            //look for next match
                            tldStart = upperWord.IndexOf("." + tld, tldStart + 1);
                        }
                    }

            return res;
        }

        public static T Hyphenate<T>(this T _paragraph, HyphenationAuto _hyph) where T : IElement
        {
            if (_paragraph is Chunk)
            {
                var chk = (Chunk)(object)_paragraph;
                chk.SetHyphenation(_hyph);
                chk.Chunks.ToList().ForEach(c => c.Hyphenate(_hyph));
            }
            else if (_paragraph is Paragraph)
            {
                var chk = (Paragraph)(object)_paragraph;

                //CLR CRASH: (!!!!)
                //chk.Chunks.ToList().ForEach(c => c.Hyphenate(_hyph));

                chk.Chunks.ToList().ForEach(c => c.SetHyphenation(_hyph));
            }
            else if (_paragraph is Anchor)
            {
                var chk = (Anchor)(object)_paragraph;
                chk.Chunks.ToList().ForEach(c => c.SetHyphenation(_hyph));
                chk.Chunks.ToList().ForEach(c => c.Hyphenate(_hyph));
            }
            else if (_paragraph is List)
            {
                var chk = (List)(object)_paragraph;
                chk.Chunks.ToList().ForEach(c => c.Hyphenate(_hyph));
            }

            return _paragraph;
        }

        public static IEnumerable<Paragraph> ToLinkedParagraph(this string _paragraph, Font _font,
            HyphenationAuto _hyph = null)
        {
            var paragraphs = new List<Paragraph>();

            var linkStyle = new Font(_font);
            linkStyle.Color = BaseColor.BLUE;
            linkStyle.SetStyle(Font.UNDERLINE);

            List currentList = null;
            var lines = _paragraph.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var para = new Paragraph();
                para.Font = new Font(_font);

                var currentLine = line;

                var listIndexOffset = 0;

                // Check if line represents a list.
                // If so, start new list of corresponding type (unordered, alphabetical, numerical)
                var isAlphaList = false;
                var isUnorderedList = false;
                var isNumericList = false;

                if (lines.Length >= 3)
                {
                    if (listUnordered.IsMatch(currentLine))
                    {
                        if (i < lines.Length - 1 && listUnordered.IsMatch(lines[i + 1]))
                            isUnorderedList = true;
                        if (i > 0 && listUnordered.IsMatch(lines[i - 1]))
                            isUnorderedList = true;
                    }
                    else if (listAlpha.IsMatch(currentLine))
                    {
                        if (i < lines.Length - 1 && listAlpha.IsMatch(lines[i + 1]))
                            isAlphaList = true;
                        if (i > 0 && listAlpha.IsMatch(lines[i - 1]))
                            isAlphaList = true;
                    }
                    else if (listNumeric.IsMatch(currentLine))
                    {
                        if (i < lines.Length - 1 && listNumeric.IsMatch(lines[i + 1]))
                            isNumericList = true;
                        if (i > 0 && listNumeric.IsMatch(lines[i - 1]))
                            isNumericList = true;
                    }
                }
                if (isAlphaList)
                {
                    var itemSymbol = listAlpha.Match(currentLine).Groups["index"].Value.ToLower().ToCharArray()[0];
                    listIndexOffset = itemSymbol - 'a';

                    currentLine = listAlpha.Replace(currentLine, "");

                    if (currentList == null)
                    {
                        currentList = new List(false, List.ALPHABETICAL, 10f);
                        currentList.Lowercase = true;
                        currentList.PostSymbol = ")";
                        currentList.IndentationLeft = 5f;
                    }
                }
                else if (isUnorderedList)
                {
                    currentLine = currentLine.TrimStart('*', '-', ' ');

                    if (currentList == null)
                    {
                        currentList = new List(List.UNORDERED, 10f);
                        currentList.SetListSymbol("\u2022");
                        currentList.IndentationLeft = 5f;
                    }
                }
                else if (isNumericList)
                {
                    var itemSymbol = int.Parse(listNumeric.Match(currentLine).Groups["index"].Value);
                    listIndexOffset = itemSymbol - 1;

                    currentLine = listNumeric.Replace(currentLine, "");

                    if (currentList == null)
                    {
                        currentList = new List(true, false, 10f);
                        currentList.IndentationLeft = 5f;
                    }
                }
                else
                {
                    // This line isn't part of a list.
                    // If we just reached the end of a list, save it & start a new paragraph.
                    if (currentList != null)
                    {
                        para.Add(currentList);
                        paragraphs.Add(para);
                        para = new Paragraph();
                        currentList = null;
                    }
                }

                foreach (var chk in currentLine.RecognizeURLs())
                {
                    var c = new Chunk(chk.Text, chk.URL == null ? _font : linkStyle);
                    if (_hyph != null)
                        c.SetHyphenation(_hyph);

                    if (chk.URL == null)
                        para.Add(c);
                    else
                    {
                        c.SetAnchor(chk.URL);
                        para.Add(c);
                    }
                }

                if (currentList == null)
                {
                    if (emptyLine.IsMatch(currentLine))
                        para.Add("\n");
                    
                    para.SpacingAfter = 2f;
                    paragraphs.Add(para);
                }
                else
                {
                    // We're in a list, store paragraph in the list for outputting later
                    currentList.First = 1 + listIndexOffset - currentList.Size; // Maintain user's list index
                    currentList.Add(new ListItem(para));
                }
            }

            // Last line -> List finished
            if (currentList != null)
            {
                var para = new Paragraph();
                para.Add(currentList);
                paragraphs.Add(para);
            }

            return paragraphs;
        }

        public class URLTuple
        {
            public string Text;
            public string URL;

            public URLTuple(string _a, string _b)
            {
                Text = _a;
                URL = _b;
            }

            public override string ToString()
            {
                if (URL == null)
                    return Text;
                return Text + " {" + URL + "}";
            }
        }
    }
}