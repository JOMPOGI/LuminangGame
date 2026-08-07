using UnityEngine;
using UnityEditor;

public class ApplyLevel1Translations
{
    [MenuItem("Tools/Localization/Apply Level 1 Translations")]
    public static void Apply()
    {
        UpdateNode("Kalaw_Intro", 
@"Squawk! Oh... prutas kadi dayta nga adda iti imam? Mabalin kadi nga mangalaak iti bassit? Ti panaglayap iti babaen daytoy napudot a init ti Vigan ket nakapoy unayak...

Mmm, nasam-it ken naimas! Agyamanak unay, biyahero!

Ay, sadinoman ti manners ko—siak ni Kalaw, ti kadwam ken guide mo kadagitoy a lugar!

Uray bassit... kitaem dayta daan nga anting-anting a pendant nga aginana iti barukongmo! Mariknam kadi dayta bassit a panaguni?

Ti Ilocos Language Crystal nga adda iti uneg dayta ket matmaturog.

Tapno mapno iti pigsa ken mapukaw ti turogna, masapul nga agpasyar ka iti Calle Crisologo ken makisarita kadagiti umili iti nasao a pagsasaoda.

Bayat ti panagsanaymo ken panagsursurom no kasano ti panagsao dagiti Ilocano iti inaldaw-aldaw a panagbiagda, ti timekmo ti makatulong a mangisubli iti pigsana ti crystal.

Isu a, rugiantayo iti nalaka.

Sakbay a pudno a makikadua ka kadagiti tattao ti Ilocos, masapul nga umuna a sursuruem no kasano ti panangabla kadakuada.");

        UpdateNode("Kalaw_W01_Teach", @"Rugiantayo iti maysa a simple a sao nga agusarem a masansan no makisabatka iti maysa a tao.

Iti Ilokano, ti 'kumusta' ket kayatna a saoen ti 'hello.'

Maysa dayta a napno iti kinamagkakadua a panangabla nga agusarem no makitam ti maysa a tao wenno no mangrugi ka iti sarita.

Denggem a naimbag: kumusta.

Ita, padasem nga ibagam ti 'kumusta' iti bukodmo a timek.");

        UpdateNode("Kalaw_W01_Success", @"Kumusta! Nasayaat! Nalpasmo a naimbag! Nasayaat dayta a wagas a panangabla iti maysa a tao!");

        UpdateNode("Kalaw_W02_Teach", @"Kalpasan ti panangabla iti maysa a tao, maipakitam pay ti naimbag a panangipategmo babaen ti panagdamag no kumusta isuna.

Iti Ilokano, ti 'kumusta ka?' ket kayatna a saoen ti 'how are you?'

Mabalinmo nga agusar iti daytoy no damagem ti kabagian, gayyem, kaarruba, wenno uray ti maysa a kabarom laeng.

Denggem a naimbag: kumusta ka?

Ita, padasem nga ibagam ti 'kumusta ka?' iti bukodmo a timek.");

        UpdateNode("Kalaw_W02_Success", @"Kumusta ka! Ah, kitaem dayta a ragsak! Agaramidka kadin iti baro a gayyem!");

        UpdateNode("Kalaw_W03_Teach", @"No damagen ka iti 'kumusta ka?', mabalinmo nga ibaga no kumusta ti kasasaadmo.

Iti Ilokano, ti 'nasayaat ak' ket kayatna a saoen ti 'I'm fine' wenno 'nasayaat ti kasasaadko.'

Kas pagarigan, no damagen ka no kasano ti panagdaliasatmo, mabalinmo nga sumungbat iti 'Nasayaat ak.'

Denggem a naimbag: nasayaat ak.

Ita, padasem nga ibagam ti 'nasayaat ak' iti bukodmo a timek.");

        UpdateNode("Kalaw_W03_Success", @"Nasayaat ak! Dayta ti espiritu! Itultuloymo dayta a napigsa a panagpursigi!");

        UpdateNode("Kalaw_W04_Teach", @"Kitaem ti init ti bigat a sumilnag iti ngato dagiti tuktok a tuilan iti Calle Crisologo.

Iti Ilokano, ti 'naimbag a bigat' ket kayatna a saoen ti 'good morning.'

Mabalinmo nga agusar iti daytoy no mangabla ka iti maysa a tao iti nasapa a paset ti aldaw.

Kas pagarigan, mabalinmo nga ibaga daytoy no mangabla ka kadagiti aglaklako bayat ti panangrugi da iti trabaho iti bigat.

Denggem a naimbag: naimbag a bigat.

Ita, padasem nga ibagam ti 'naimbag a bigat' iti bukodmo a timek.");

        UpdateNode("Kalaw_W04_Success", @"Naimbag a bigat! Nasursurom a nalaing ti panangabla iti bigat!

Agpayso ka ken Vendor Kyros iti souvenir stallna. Isurona kenka no kasano ti panangabla kadagiti tattao kadagiti dadduma a oras ti aldaw.");

        UpdateNode("Kyros_W05_Teach", @"Naimbag nga aldaw, biyahero! Imbaga ni Kalaw kaniak nga adda baro a biyahero nga agpasyar iti Vigan.

No nalpasen ti bigat ken dumteng ti malem, mabalin nga mangabla dagiti Ilocano iti maysa a tao babaen ti panangibaga iti 'naimbag a malem.'

Kayatna a saoen ti 'good afternoon.'

Mabalinmo nga agusar iti daytoy no mangabla ka iti maysa a tao iti malem.

Denggem a naimbag: naimbag a malem.

Ita, padasem nga ibagam ti 'naimbag a malem' iti bukodmo a timek.");
        
        UpdateNode("Kyros_W05_Success", @"Naimbag a malem! Umayka iti tindak, gayyem!");

        UpdateNode("Kyros_W06_Teach", @"No lumubog ti init ken mangrugi a sumilnag dagiti silaw iti dalan, agbaliw ti panangabla segun iti oras ti aldaw.

Iti Ilokano, ti 'naimbag a rabii' ket kayatna a saoen ti 'good evening.'

Usarem daytoy no mangabla ka iti maysa a tao iti rabii.

Denggem a naimbag: naimbag a rabii.

Ita, padasem nga ibagam ti 'naimbag a rabii' iti bukodmo a timek.");
        UpdateNode("Kyros_W06_Success", @"Naimbag a rabii! Kitaem, sumilnag ti pendantmo!");

        UpdateNode("Kyros_W07_Teach", @"Adda dagiti oras a kayatmo laeng a mangted iti sapasap a panangabla iti maysa a tao nga awan ti espesipiko a bigat, malem, wenno rabii.

Iti Ilokano, ti 'naimbag nga aldaw' ket kayatna a saoen ti 'good day.'

Maysa dayta a sapasap a panangabla nga agusarem no kayatmo a tarigagayen ti naimbag nga aldaw ti maysa a tao.

Denggem a naimbag: naimbag nga aldaw.

Ita, padasem nga ibagam ti 'naimbag nga aldaw' iti bukodmo a timek.");
        UpdateNode("Kyros_W07_Success", @"Naimbag nga aldaw! Sapay koma ta ragsakam ti panagpasyarmo iti Calle Crisologo!");

        UpdateNode("Kyros_W08_Teach", @"Ti tunggal dalan ket agtultuloy iti sabali a lugar.

No pumanawka iti maysa a tao, mabalinmo nga ibaga ti 'agpakada akon' a kayatna a saoen ti 'goodbye.'

Usarem daytoy no pagpatinggaem ti sarita wenno no agsinaaykayo.

Denggem a naimbag: agpakada akon.

Ita, padasem nga ibagam ti 'agpakada akon' iti bukodmo a timek.");
        UpdateNode("Kyros_W08_Success", @"Agpakada akon! ✨ NALUKATAN TI GREETINGS MILESTONE! ✨

Mapanka ken Vendor Irah iti loom a panagabel. Adda napateg nga isurona kenka maipapan iti panangipakita iti panagyaman.");

        UpdateNode("Irah_W09_Teach", @"Clack-clack-clack ti loom! Naragsak a panagbalinka ditoy, biyahero!

No adda tumulong kenka, nangted iti maysa a banag kenka, wenno nangaramid iti naimbag a banag para kenka, napateg nga ipakitam ti panagyamanmo.

Iti Ilokano, ti 'agyamanak' ket kayatna a saoen ti 'thank you.'

Mabalinmo nga agusar iti daytoy no kayatmo nga ipakita ti panagyamanmo.

Denggem a naimbag: agyamanak.

Ita, padasem nga ibagam ti 'agyamanak' iti bukodmo a timek.");
        UpdateNode("Irah_W09_Success", @"Agyamanak! Ti panangipakita iti panagyaman ket makatulong a mangpapigsa iti panagkakayamet ti komunidad!");

        UpdateNode("Irah_W10_Teach", @"Adda dagiti oras a saan a makaanay ti simple a panagyaman tapno maipakita no kasano ti kaadu ti panagyamanmo.

Iti Ilokano, ti 'agyamanak unay' ket kayatna a saoen ti 'thank you very much.'

Mabalinmo nga agusar iti daytoy no adda nangaramid iti naisangsangayan a naimbag wenno naindaklan a tulong kenka.

Denggem a naimbag: agyamanak unay.

Ita, padasem nga ibagam ti 'agyamanak unay' iti bukodmo a timek.");
        UpdateNode("Irah_W10_Success", @"Agyamanak unay! Sapay koma ta nalaka ken naraniag ti dalan ti panagdaliasatmo!");

        UpdateNode("Irah_W11_Teach", @"No adda nangted kenka iti tulong, mabalinmo nga agyaman iti espesipiko a tulong nga inaramidna.

Iti Ilokano, ti 'agyamanak iti tulong mo' ket kayatna a saoen ti 'thank you for your help.'

Mabalinmo nga agusar iti daytoy no adda tumulong kenka a mangileppas iti maysa a trabaho wenno mangrisot iti maysa a parikut.

Denggem a naimbag: agyamanak iti tulong mo.

Ita, padasem nga ibagam ti 'agyamanak iti tulong mo' iti bukodmo a timek.");
        UpdateNode("Irah_W11_Success", @"Agyamanak iti tulong mo! Ay-ayatenmi dagiti Ilocano ti panagtutulong iti tunggal maysa!");

        UpdateNode("Irah_W12_Teach", @"Oops! Agannadka kadagitoy nadumaduma a linya.

No saanmo a ninamnama a nasangkam ti maysa a tao wenno nakaaramidka iti biddut, mabalinmo nga agpakawan.

Iti Ilokano, ti 'pakawanen nak' ket kayatna a saoen ti 'I am sorry.'

Usarem daytoy no kayatmo nga agpakawan iti maysa a banag nga inaramidmo.

Denggem a naimbag: pakawanen nak.

Ita, padasem nga ibagam ti 'pakawanen nak' iti bukodmo a timek.");
        UpdateNode("Irah_W12_Success", @"Pakawanen nak... Awan ti problema!

Mapanka ken Vendor Jom iti empanada stallna para iti maudi a pagsasao ti gratitude!");

        UpdateNode("Jom_W13_Teach", @"Mariknam kadi ti angot dayta a mapudpudot a longganisa? Napno ti counterko!

No kasapulam a tawagan iti naurnos a wagas ti panagkitana ti maysa a tao wenno lumabas iti napno a lugar, mabalinmo nga ibaga ti 'dispensaren nak.'

Kayatna a saoen ti 'excuse me.'

Denggem a naimbag: dispensaren nak.

Ita, padasem nga ibagam ti 'dispensaren nak' iti bukodmo a timek.");
        UpdateNode("Jom_W13_Success", @"Dispensaren nak! Nasayaat ti mannersmo!

✨ NALUKATAN TI GRATITUDE MILESTONE! ✨

Agtalinaedka pay para kadagiti Response trials!");

        UpdateNode("Jom_W14_Teach", @"Agpraktistayo iti maysa a simple a sungbat.

No adda mangdamag kenka iti maysa a saludsod ken kayatmo nga umanamong wenno sumungbat iti 'yes,' agusar ti Ilokano iti sao a 'wen.'

Denggem a naimbag: wen.

Ita, padasem nga ibagam ti 'wen' iti bukodmo a timek.");
        UpdateNode("Jom_W14_Success", @"Wen! Dayta ti kayatko a denggen! Agtultuloy ti panagluto!");

        UpdateNode("Jom_W15_Teach", @"Natural, adda met dagiti oras a kasapulam nga agsungbat iti 'no' wenno saan nga umanamong.

Iti Ilokano, ti 'saan' ket kayatna a saoen ti 'no.'

Mabalinmo nga agusar iti daytoy no kayatmo nga mangted iti negatibo a sungbat.

Denggem a naimbag: saan.

Ita, padasem nga ibagam ti 'saan' iti bukodmo a timek.");
        UpdateNode("Jom_W15_Success", @"Saan! Naawatak! Isaganaak laeng dayta a garlic vinegar iti sabali nga aldaw!");

        UpdateNode("Jom_W16_Teach", @"Adda dagiti oras a kayatmo laeng a mangipakita nga umanamongka wenno naawatam ti maysa a banag.

Iti Ilokano, mabalin met nga agusar iti 'okay' tapno ibagam ti 'okay.'

Maysa dayta a simple a sungbat nga agusarem tapno ipakitam nga naawatam ti banag wenno umanamongka.

Denggem a naimbag: okay.

Ita, padasem nga ibagam ti 'okay' iti bukodmo a timek.");
        UpdateNode("Jom_W16_Success", @"Okay! Nalaka laeng! Napardas ti panagsursurom!");

        UpdateNode("Jom_W17_Teach", @"No adda mangilawlawag iti maysa a banag ken kayatmo nga ibaga nga naawatam, mabalinmo nga agusar iti 'maawatan ko.'

Kayatna a saoen ti 'I understand.'

Denggem a naimbag: maawatan ko.

Ita, padasem nga ibagam ti 'maawatan ko' iti bukodmo a timek.");
        UpdateNode("Jom_W17_Success", @"Maawatan ko! Naimbag unay! Agtultuloy ti panangawatmo!");

        UpdateNode("Jom_W18_Teach", @"No adda nagsao iti maysa a banag nga saanmo a naawatan, nasayaat nga ibagam kadakuada.

Iti Ilokano, ti 'diak maawatan' ket kayatna a saoen ti 'I don't understand.'

Mabalinmo nga agusar iti daytoy no kasapulam nga ilawlawag manen ti maysa a tao.

Denggem a naimbag: diak maawatan.

Ita, padasem nga ibagam ti 'diak maawatan' iti bukodmo a timek.");
        UpdateNode("Jom_W18_Success", @"Diak maawatan! Awan problema—masapul ti praktis iti panagsursuro!

✨ NALUKATAN TI RESPONSES MILESTONE! ✨");

        UpdateNode("Ronnie_W19_Teach", @"No makisabatka iti baro a tao, maysa kadagiti umuna a banag nga kayatmo a maammoan ket ti naganna.

Iti Ilokano, ti 'ania ti nagan mo?' ket kayatna a saoen ti 'what is your name?'

Mabalinmo nga agusar iti daytoy a pagsasao no mangrugi ka nga agpakilala iti bagim wenno no damagem ti nagan ti maysa a tao.

Denggem a naimbag: ania ti nagan mo?

Ita, padasem nga ibagam ti 'ania ti nagan mo?' iti bukodmo a timek.");
        UpdateNode("Ronnie_W19_Success", @"Ania ti nagan mo? Nasayaat dayta a saludsod no makisabatka iti baro a tao!");

        UpdateNode("Ronnie_W20_Teach", @"Ita, sika met ti agpakilala iti bagim.

Iti Ilokano, ti 'ti nagan ko ket...' ket kayatna a saoen ti 'my name is...'

Dagiti sao a 'ti nagan ko ket' ket isu ti fixed a paset ti pagsasao. Kalpasanna, iyungetmo ti bukodmo a nagan.

Kas pagarigan: 'Ti nagan ko ket Jom.'

Denggem a naimbag: ti nagan ko ket Jom.

Ita, padasem nga agpakilala iti bagim babaen ti 'ti nagan ko ket...' ken iyungetmo ti bukodmo a nagan.");
        UpdateNode("Ronnie_W20_Success", @"Nasayaat! Naragsakak a nakisabat kenka! Nagpakilalaka iti Ilokano!

Mapanka ken Sally iti asideg ti brick arch tapno malpasmo ti Level I!");

        UpdateNode("Sally_W21_Teach", @"No makisabatka iti baro a tao, mabalinmo met a kayat a maammoan no sadino ti naggapuan na.

Iti Ilokano, ti 'taga sadino ka?' ket kayatna a saoen ti 'where are you from?'

Usarem daytoy no damagem no sadino ti ili wenno lugar a naggapuanna.

Denggem a naimbag: taga sadino ka?

Ita, padasem nga ibagam ti 'taga sadino ka?' iti bukodmo a timek.");
        UpdateNode("Sally_W21_Success", @"Taga sadino ka? Dayta ti wagas a panangdamag no sadino ti naggappuan ti maysa a tao!");

        UpdateNode("Sally_W22_Teach", @"Ita, mabalinmo nga sumungbat iti dayta a saludsod.

Iti Ilokano, ti 'taga ___ ak' ket kayatna a saoen ti 'I am from ___.'

Ti sao a 'taga' ket mangipakita iti lugar a naggappuam, bayat a agbaliw ti lugar segun iti sungbatmo.

Kas pagarigan: 'taga Vigan ak.'

Denggem a naimbag: taga Vigan ak.

Ita, padasem nga ibagam ti 'taga' ken kalpasanna ti nagan ti ili wenno lugar a naggappuam.");
        UpdateNode("Sally_W22_Success", @"Nasayaat! Imbagam laeng no sadino ti naggappuam!

🏆 NALPAS TI LEVEL I: CONVERSATIONAL & SOCIAL! 🏆

Nasursurom no kasano ti mangabla kadagiti tattao, mangipakita iti panagyaman, sumungbat kadagiti dadduma, ken agpakilala iti bagim.

Ita, agtultuloy ti panagdaliasatmo iti ad-adu pay a paset ti inaldaw-aldaw a panagkomunikar.

Wen, umayka iti Level II: Functional & Navigational!");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=yellow>[Level 1] Updated 22 dialogue nodes with translations!</color>");
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
                // Push current English text into translatedText
                if (string.IsNullOrEmpty(node.translatedText))
                {
                    node.translatedText = node.dialogueText;
                }
                
                // Set main text to Ilokano
                node.dialogueText = ilokanoText;
                EditorUtility.SetDirty(node);
            }
        }
    }
}
