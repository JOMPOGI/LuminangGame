using UnityEngine;
using UnityEditor;

public class ApplyLevel3Translations
{
    [MenuItem("Tools/Localization/Apply Level 3 Translations")]
    public static void ApplyManual()
    {
        Apply();
    }

    [InitializeOnLoadMethod]
    private static void AutoRun()
    {
        if (!EditorPrefs.GetBool("Level3TranslationsApplied", false))
        {
            Apply();
            EditorPrefs.SetBool("Level3TranslationsApplied", true);
        }
    }

    private static void Apply()
    {
        UpdateNode("MangLance_W48_Teach", @"Kalpasan ti amin a panagpaspas ken panagpasyar, nalabit nga mabisinan ka!

Iti Ilokano, ti 'mangan' ket kayatna a saoen ti 'eat.'

Maysa dayta nga action verb nga agusar no pagsasaritaan ti aramid a panangan.

Kas pagarigan, no orasmon ti panangan iti pangrabii, mabalinmo nga ibaga ti 'mangan.'

Denggem a naimbag: mangan.

Ita, padasem nga ibagam ti 'mangan' iti bukodmo a timek.");
        UpdateNode("MangLance_W48_Success", @"Mangan! Pagbalinen nak a mabisin!

Mapanka ken Rayo, ti photographer, para kadagiti sumaruno nga action verbs!");

        UpdateNode("Rayo_W49_Teach", @"Click! Nasayaat a pose!

Napudot ita nga aldaw, ket nalabit a kayatmo ti maysa a makapresko.

Iti Ilokano, ti 'uminom' ket kayatna a saoen ti 'drink.'

Maysa dayta nga action verb nga agusar no pagsasaritaan ti panaginom iti maysa a banag.

Denggem a naimbag: uminom.

Ita, padasem nga ibagam ti 'uminom' iti bukodmo a timek.");
        UpdateNode("Rayo_W49_Success", @"Uminom! Makapresko!");

        UpdateNode("Rayo_W50_Teach", @"No kayatmo a pagsaritaan ti panaggunay manipud iti maysa a lugar agingga iti sabali a lugar, mabalinmo nga agusar iti verb a 'mapan.'

Kayatna a saoen ti 'go.'

Kas pagarigan, no mapanka iti maysa a napintas a lugar a pagpicturean, mabalinmo nga ibaga ti 'mapan.'

Denggem a naimbag: mapan.

Ita, padasem nga ibagam ti 'mapan' iti bukodmo a timek.");
        UpdateNode("Rayo_W50_Success", @"Mapan! Mapantayo iti arko!");

        UpdateNode("Rayo_W51_Teach", @"Ita, sursuruentayo ti kasumbalikna a direksyon.

Iti Ilokano, ti 'umay' ket kayatna a saoen ti 'come.'

Mabalinmo nga agusar iti daytoy no kayatmo a dawaten iti maysa a tao nga umay iti ayanmo wenno iti espesipiko a lugar.

Denggem a naimbag: umay.

Ita, padasem nga ibagam ti 'umay' iti bukodmo a timek.");
        UpdateNode("Rayo_W51_Success", @"Umay! Umayka ken agtakderka iti abay ti tawa!");

        UpdateNode("Rayo_W52_Teach", @"Kalpasan ti panagpasyar iti intero nga aldaw, kasapulan ti tunggal maysa ti tiempo a paginana.

Iti Ilokano, ti 'maturog' ket kayatna a saoen ti 'sleep.'

Dayta ti aramid ti panaginana bayat ti turog.

Denggem a naimbag: maturog.

Ita, padasem nga ibagam ti 'maturog' iti bukodmo a timek.");
        UpdateNode("Rayo_W52_Success", @"Maturog a naimbag!

Mapanka ken Aling Rosa tapno malpasmo dagiti Action Verbs!");

        UpdateNode("AlingRosa_Intro", @"Ay, napukaw ti linya!

Pangngaasim, sapulem ti bassit a linya tapno malpas ko ti panagabel kadagitoy napintas ken nadumaduma a souvenir!");

        UpdateNode("AlingRosa_W53_Teach", @"Aww, agyamanak!

No kitaem ti maysa a banag wenno mapaliiwmo iti mata, ti verb iti Ilokano a 'makita' ket kayatna a saoen ti 'see.'

Kas pagarigan, mabalinmo nga agusar iti daytoy no kitaem dagitoy naraniag a tela.

Denggem a naimbag: makita.

Ita, padasem nga ibagam ti 'makita' iti bukodmo a timek.");
        UpdateNode("AlingRosa_W53_Success", @"Makita! Kitaem dagitoy napintas a disenyo!");

        UpdateNode("AlingRosa_W54_Teach", @"Denggem a naimbag. Mangngegmo kadi dagiti yapak dagiti kabalyo nga agpaspas iti Calle Crisologo?

Iti Ilokano, ti 'mangngeg' ket kayatna a saoen ti 'hear.'

Denggem a naimbag: mangngeg.

Ita, padasem nga ibagam ti 'mangngeg' iti bukodmo a timek.");
        UpdateNode("AlingRosa_W54_Success", @"Mangngeg! Mangngegmo dagiti kalesa nga umay!");

        UpdateNode("AlingRosa_W55_Teach", @"Agaramidka iti napateg unay ita—agusarka iti timekmo.

Iti Ilokano, ti 'agsao' ket kayatna a saoen ti 'speak.'

Mabalinmo nga agusar iti daytoy no pagsasaritaan ti panagkisarita iti sabali a tao wenno no agpraktiska iti maysa a pagsasao.

Denggem a naimbag: agsao.

Ita, padasem nga ibagam ti 'agsao' iti bukodmo a timek.");
        UpdateNode("AlingRosa_W55_Success", @"Agsao!

✨ NALUKATAN TI ACTION VERBS MILESTONE! ✨

Agtalinaedka ditoy para kadagiti Linking Verbs!");

        UpdateNode("AlingRosa_W56_Teach", @"Ita, mapantayo iti ad-adu a nasukimat a paset ti Ilokano.

Iti maysa a pangungusap a kas iti 'Siak ket weaver,' ti sao a 'ket' ket mangikonekta iti subject iti impormasyon a mangiladawan kenkuana.

Ibilangmo ti 'ket' a kas maysa a connector nga makatulong a mangikonekta iti dua a paset ti pangungusap.

Denggem a naimbag: ket.

Ita, padasem nga ibagam ti 'ket' iti bukodmo a timek.");
        UpdateNode("AlingRosa_W56_Success", @"Ket! Nasakto!

Mapanka ken Lola Nida para kadagiti ad-adu pay a connector words!");

        UpdateNode("LolaNida_W57_Teach", @"No mangibaga wenno mangiladawan ka iti espesipiko a banag, mabalin nga agusar ti Ilokano iti 'isu ti' a paset ti istruktura ti pangungusap.

Kas pagarigan, makatulong daytoy a mangipakita iti kapanunotan a kas iti 'is the' iti English.

Denggem a naimbag: isu ti.

Ita, padasem nga ibagam ti 'isu ti' iti bukodmo a timek.");
        UpdateNode("LolaNida_W57_Success", @"Isu ti! Sursuruem no kasano nga ikonekta ti Ilokano dagiti kapanunotan!");

        UpdateNode("LolaNida_W58_Teach", @"No pagsasaritaanmo ti grupo dagiti tattao, mabalin nga agusar ti Ilokano iti 'da' kas plural marker.

Kas pagarigan, no pagsasaritaanmo ti maysa a grupo kas dagiti agabel, makatulong ti 'da' a mangipakita nga ad-adu ngem iti maysa a tao ti pagsasaritaanmo.

Denggem a naimbag: da.

Ita, padasem nga ibagam ti 'da' iti bukodmo a timek.");
        UpdateNode("LolaNida_W58_Success", @"Da! Sursuruem no kasano nga ipakita ti Ilokano dagiti grupo!");

        UpdateNode("LolaNida_W59_Teach", @"No pagsasaritaanmo ti maysa a banag a nagadda wenno pudno idi napalabas, mabalinmo nga agusar iti 'ket idi' iti konteksto ti pakasaritaan.

Kas pagarigan, no iladawanmo no kasano ti maysa a lugar idi napalabas, makatulong daytoy a pagsasao a mangikonekta iti subject iti daan a kasasaadna.

Denggem a naimbag: ket idi.

Ita, padasem nga ibagam ti 'ket idi' iti bukodmo a timek.");
        UpdateNode("LolaNida_W59_Success", @"Ket idi! Ikonektaem ti pagsasao iti pakasaritaan ti Ilocos!");

        UpdateNode("LolaNida_W60_Teach", @"Daytoy met laeng a pagsasao mausar iti panagsasarita maipapan iti grupo iti konteksto ti pakasaritaan.

Kas pagarigan, no iladawanmo no kasano dagiti tattao idi napalabas, ti 'ket idi' ket mabalin a paset ti istruktura ti pangungusap.

Denggem a naimbag: ket idi.

Ita, padasem nga ibagam ti 'ket idi' iti bukodmo a timek.");
        UpdateNode("LolaNida_W60_Success", @"Ket idi! Agtultuloy ti napalabas babaen kadagiti istoria nga isasaritatayo!");

        UpdateNode("Neneng_W61_Teach", @"Hey! Adda maysa a napintas a banag ditoy.

Iti Ilokano, ti 'agbalin' ket kayatna a saoen ti 'become.'

Iladladawanna ti panagbaliw manipud iti maysa a kasasaad wenno kondisyon iti sabali.

Kas pagarigan, babaen ti praktis, mabalinmo nga agbalin a nasaysayaat ken natalek iti panagsao iti Ilokano.

Denggem a naimbag: agbalin.

Ita, padasem nga ibagam ti 'agbalin' iti bukodmo a timek.");
        UpdateNode("Neneng_W61_Success", @"Agbalin! Babaen ti praktis, mabalinmo nga agbalin a natalek nga agsao iti Ilokano!");

        UpdateNode("Neneng_W62_Teach", @"Adda dagiti oras a kayattayo a iladawan no kasano ti langa wenno rikna ti maysa a banag kadatayo.

Iti Ilokano, ti 'kasla' ket mabalin a mangipakita iti kapanunotan a 'seem' wenno 'like.'

Kas pagarigan, mabalinmo a saoen nga kasla maysa a panagdaliasat iti napalabas ti tiempo ti maysa a dalan.

Denggem a naimbag: kasla.

Ita, padasem nga ibagam ti 'kasla' iti bukodmo a timek.");
        UpdateNode("Neneng_W62_Success", @"Kasla! Pudno unay a kasla agsubli ti panagdaliasat iti pakasaritaan!");

        UpdateNode("Neneng_W63_Teach", @"Adda dagiti banag a agbaliw, ngem adda met dagiti agtalinaed a napigsa.

Iti Ilokano, ti 'agtalinaed' ket kayatna a saoen ti 'remain.'

Mabalinmo nga agusar iti daytoy no pagsasaritaanmo ti maysa a banag nga agtultuloy a nagnaed wenno agtalinaed iti maysa a kasasaad.

Denggem a naimbag: agtalinaed.

Ita, padasem nga ibagam ti 'agtalinaed' iti bukodmo a timek.");
        UpdateNode("Neneng_W63_Success", @"Agtalinaed! Sapay koma ta agtalinaed a napigsa ti kultura tayo iti sumarsaruno a henerasyon!");

        UpdateNode("Neneng_W64_Teach", @"No pagsasaritaanmo ti panagtalinaed wenno panagnaed iti maysa a lugar, agusar ti Ilokano iti 'agyan.'

Mabalinmo nga agusar iti daytoy no pagsasaritaanmo no sadino ti pagnaedan wenno pagtalinaedan ti maysa a tao.

Denggem a naimbag: agyan.

Ita, padasem nga ibagam ti 'agyan' iti bukodmo a timek.");
        UpdateNode("Neneng_W64_Success", @"Agyan! Sapay koma ta agtalinaedka pay iti Vigan iti ad-adu pay nga aldaw!

Mapanka ken Aling Riza iti restaurant!");

        UpdateNode("AlingRiza_W65_Teach", @"Ti pagsasao ket saan laeng a maipapan kadagiti sasao. No dadduma, maipapan met iti rikna ken kapadasanmo.

Iti Ilokano, ti 'marikna' ket kayatna a saoen ti 'feel' wenno 'perceive.'

Mabalinmo nga agusar iti daytoy no pagsasaritaanmo ti panagkapadas iti maysa a banag babaen kadagiti riknam wenno emosyonmo.

Denggem a naimbag: marikna.

Ita, padasem nga ibagam ti 'marikna' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W65_Success", @"Marikna!

✨ NALUKATAN TI LINKING VERBS MILESTONE! ✨

Makisaritaka manen kaniak para kadagiti Pronouns!");

        UpdateNode("AlingRiza_W66_Teach", @"Sursuruentayo dagiti sasao a makatulong kadatayo a mangsarita maipapan kadagiti tattao.

Iti Ilokano, ti 'siak' ket kayatna a saoen ti 'I.'

Usarem daytoy no pagsasaritaanmo ti bagim.

Denggem a naimbag: siak.

Ita, padasem nga ibagam ti 'siak' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W66_Success", @"Siak! Sika dayta!");

        UpdateNode("AlingRiza_W67_Teach", @"No makisaritaka a direkta iti sabali a tao, agusar ti Ilokano iti 'sika' para iti 'you.'

Denggem a naimbag: sika.

Ita, padasem nga ibagam ti 'sika' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W67_Success", @"Sika! Dayta ti sao para iti tao nga pagsasaritaam!");

        UpdateNode("AlingRiza_W68_Teach", @"No pagsasaritaanmo ti lalaki a saan nga isuna ti agsao wenno ti tao nga pagsasaritaam, agusar ti Ilokano iti 'isuna' para iti 'he.'

Denggem a naimbag: isuna.

Ita, padasem nga ibagam ti 'isuna' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W68_Success", @"Isuna! Pagsasaritaanmo ti sabali a tao!");

        UpdateNode("AlingRiza_W69_Teach", @"Napateg a maammoan a ti isu met laeng a pronoun iti Ilokano a 'isuna' ket mabalin met a mangitukoy iti 'she.'

Ti kayatna a saoen ket agdepende iti tao nga pagsasaritaanmo ken iti konteksto.

Denggem a naimbag: isuna.

Ita, padasem nga ibagam ti 'isuna' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W69_Success", @"Isuna! Ti konteksto ti makatulong a mangawat no asino ti pagsasaritaan!");

        UpdateNode("AlingRiza_W70_Teach", @"No pagsasaritaanmo ti bagim a kadua dagiti dadduma, mabalinmo nga agusar iti 'dakkami' para iti 'we' iti konteksto nga sursursuruentayo.

Denggem a naimbag: dakkami.

Ita, padasem nga ibagam ti 'dakkami' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W70_Success", @"Dakkami! Ita, agsarsaritaka kas paset ti maysa a grupo!");

        UpdateNode("AlingRiza_W71_Teach", @"No pagsasaritaanmo ti maysa a grupo dagiti sabali a tattao, agusar ti Ilokano iti 'isuda' para iti 'they.'

Denggem a naimbag: isuda.

Ita, padasem nga ibagam ti 'isuda' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W71_Success", @"Isuda! Pagsasaritaanmo ida!");

        UpdateNode("AlingRiza_W07_Teach", @"Ti Ilokano ket mabalin nga agusar iti 'siak' no mangitukoy iti 'me,' depende iti istruktura ti pangungusap.

Denggem a naimbag: siak.

Ita, padasem nga ibagam ti 'siak' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W07_Success", @"Siak! Agbalinmon a komportable kadagitoy a pronouns!");

        UpdateNode("AlingRiza_W73_Teach", @"No mangitukoyka iti bagim a kadua dagiti dadduma, ti 'dakkami' ket mabalin a mangitakder iti 'us' iti konteksto nga sursursuruentayo.

Denggem a naimbag: dakkami.

Ita, padasem nga ibagam ti 'dakkami' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W73_Success", @"Dakkami! Ti pagsasao ket makatulong a mangiladawan no asino dagiti agkakadua!");

        UpdateNode("AlingRiza_W74_Teach", @"No mangitukoyka kadagiti sabali a tattao kas pakayatan ti maysa nga aramid, ti 'isuda' ti mausar iti konteksto nga sursursuruentayo para iti 'them.'

Denggem a naimbag: isuda.

Ita, padasem nga ibagam ti 'isuda' iti bukodmo a timek.");
        UpdateNode("AlingRiza_W74_Success", @"Isuda! Nasursurom no kasano nga pagsasaritaan ti Ilokano dagiti nadumaduma a tattao!");

        UpdateNode("LolaBebang_W75_Teach", @"Dagiti saludsod ket makatulong kadatayo a mangammo iti lubong iti aglawlawtayo.

Iti Ilokano, ti 'ania' ket kayatna a saoen ti 'what.'

Usarem daytoy no damagem maipapan iti maysa a banag, bagay, wenno impormasyon.

Denggem a naimbag: ania.

Ita, padasem nga ibagam ti 'ania' iti bukodmo a timek.");
        UpdateNode("LolaBebang_W75_Success", @"Ania! Nakapadas ka itan a mangdamag iti saludsod!");

        UpdateNode("LolaBebang_W76_Teach", @"No kayatmo a maammoan no asino ti pagsasaritaanmo, agusarka iti 'asino.'

Kayatna a saoen ti 'who.'

Denggem a naimbag: asino.

Ita, padasem nga ibagam ti 'asino' iti bukodmo a timek.");
        UpdateNode("LolaBebang_W76_Success", @"Asino! Ita, mabalinmon a damagen no asino ti maysa a tao!");

        UpdateNode("LolaBebang_W77_Teach", @"No kasapulam a sapulen ti maysa a lugar wenno lokasyon, mabalinmo a damagen ti 'where?'

Iti Ilokano, ti 'sadino' ket kayatna a saoen ti 'where.'

Denggem a naimbag: sadino.

Ita, padasem nga ibagam ti 'sadino' iti bukodmo a timek.");
        UpdateNode("LolaBebang_W77_Success", @"Sadino! Ita, mabalinmon a damagen no sadino ti ayan ti maysa a banag!");

        UpdateNode("LolaBebang_W78_Teach", @"No damagem maipapan iti tiempo wenno oras, mabalinmo nga agusar iti 'kaano.'

Kayatna a saoen ti 'when.'

Denggem a naimbag: kaano.

Ita, padasem nga ibagam ti 'kaano' iti bukodmo a timek.");
        UpdateNode("LolaBebang_W78_Success", @"Kaano! Ita, mabalinmon a damagen no kaano a napasamak ti maysa a banag!");

        UpdateNode("LolaBebang_W79_Teach", @"Adda dagiti oras a kayattayo a maawatan ti rason iti likudan ti maysa a banag.

Iti Ilokano, ti 'apay' ket kayatna a saoen ti 'why.'

Usarem daytoy no damagem ti rason wenno ilawlawag ti maysa a banag.

Denggem a naimbag: apay.

Ita, padasem nga ibagam ti 'apay' iti bukodmo a timek.");
        UpdateNode("LolaBebang_W79_Success", @"Apay! Ita, mabalinmon a damagen no apay!");

        UpdateNode("LolaBebang_W80_Teach", @"No kayatmo a damagen no kasano a naaramid ti maysa a banag, agusarka iti 'kasano.'

Kayatna a saoen ti 'how.'

Denggem a naimbag: kasano.

Ita, padasem nga ibagam ti 'kasano' iti bukodmo a timek.");
        UpdateNode("LolaBebang_W80_Success", @"Kasano! Kitaem no kasano ti kinadayo ti nadanonmo!");

        UpdateNode("LolaBebang_W81_Teach", @"Ket ita, nakadanonka iti sabali pay a napateg a saludsod.

No kayatmo a damagen ti bilang wenno kadakkel ti maysa a grupo, agusar ti Ilokano iti 'mano' para iti 'how many.'

Mabalinmo nga agusar iti daytoy no damagem ti bilang dagiti tattao wenno banag.

Denggem a naimbag: mano.

Ita, padasem nga ibagam ti 'mano' iti bukodmo a timek.");
        UpdateNode("LolaBebang_W81_Success", @"Mano! Ita, mabalinmon a damagen ti bilang dagiti tattao ken banag.

Nasursurom no kasano ti makikomunikar iti adu a kasasaad iti inaldaw-aldaw iti amin a panagdaliasatmo.

Ngem saan a nagpatingga ti panagsursuro babaen laeng iti panangulit iti nadenggem.

Agsublika ken Kalaw iti plaza.

Adda agur-uray kenka a maudi a karit.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=yellow>[Level 3] Updated 69 dialogue nodes with translations!</color>");
    }

    private static void UpdateNode(string assetName, string ilokanoText)
    {
        string[] guids = AssetDatabase.FindAssets(assetName + " t:DialogueNode", new[] { "Assets/Dialogues/CalleCrisologo" });
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            if (node != null)
            {
                if (string.IsNullOrEmpty(node.translatedText))
                {
                    node.translatedText = node.dialogueText;
                }
                node.dialogueText = ilokanoText;
                EditorUtility.SetDirty(node);
            }
        }
    }
}
