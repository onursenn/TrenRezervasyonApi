using Microsoft.AspNetCore.Mvc;
using TrenRezervasyonApi.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;


namespace TrenRezervasyonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrenRezervasyonApi : ControllerBase
    {
        [HttpPost(Name = "TrenRezervasyonApi")]
        public DonusFormati Post(TrenRezIslem trenRez)
        {
            DonusFormati donus = new DonusFormati();
            List<YerlesimAyrinti> yerlesim = new List<YerlesimAyrinti>();

            if (trenRez.KisilerFarkliVagonlaraYerlestirilebilir)
            {
                int yolcuSayisi = trenRez.RezervasyonYapilacakKisiSayisi;

                foreach (var item in trenRez.Tren.Vagonlar)
                {
                    int vagonKalanBosYer = Convert.ToInt32(item.Kapasite * 0.7 - item.DoluKoltukAdet);

                    int vagonayerlesecekkisisayisi = 0;

                    if (vagonKalanBosYer > 0 && yolcuSayisi > 0)
                    {
                        if (vagonKalanBosYer > yolcuSayisi)
                        {
                            vagonayerlesecekkisisayisi = yolcuSayisi;
                            yolcuSayisi = 0;
                        }
                        else
                        {
                            vagonayerlesecekkisisayisi = vagonKalanBosYer;
                            yolcuSayisi -= vagonKalanBosYer;
                        }
                        yerlesim.Add(new YerlesimAyrinti { KisiSayisi = vagonayerlesecekkisisayisi, VagonAdi = item.Ad });
                    }
                }
                if (yolcuSayisi == 0)
                {
                    donus.YerlesimAyrinti = yerlesim;
                    donus.RezervasyonYapabilir = true;
                    return donus;
                }
            }
            else
            {
                foreach (var item in trenRez.Tren.Vagonlar)
                {
                    int vagonKalanBosYer = Convert.ToInt32(item.Kapasite * 0.7 - item.DoluKoltukAdet);

                    if (trenRez.RezervasyonYapilacakKisiSayisi <= vagonKalanBosYer)
                    {
                        yerlesim.Add(new YerlesimAyrinti { KisiSayisi = trenRez.RezervasyonYapilacakKisiSayisi, VagonAdi = item.Ad });
                        donus.YerlesimAyrinti = yerlesim;
                        donus.RezervasyonYapabilir = true;
                        return donus;
                    }
                }
            }
            donus.YerlesimAyrinti = new List<YerlesimAyrinti>();
            return donus;
        }
    }
}
