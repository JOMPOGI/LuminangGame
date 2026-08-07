using UnityEngine;
using UnityEditor;

public class ApplyLevel2Translations
{
    [MenuItem("Tools/Localization/Apply Level 2 Translations")]
    public static void ApplyManual()
    {
        Apply();
    }

    [InitializeOnLoadMethod]
    private static void AutoRun()
    {
        if (!EditorPrefs.GetBool("Level2TranslationsApplied", false))
        {
            Apply();
            EditorPrefs.SetBool("Level2TranslationsApplied", true);
        }
    }

    private static void Apply()
    {
        UpdateNode("Sally_W23_Teach", @"No adda problema wenno kasapulam ti tulong, masapulmo ti nalawag a wagas a panagkiddaw iti tulong.

Iti Ilokano, ti 'tulunganak' ket kayatna a saoen ti 'help me.'

Usarem daytoy no kasapulam a dagus ti tulong ti maysa a tao.

Denggem a naimbag: tulunganak.

Ita, padasem nga ibagam ti 'tulunganak' iti bukodmo a timek.");
        UpdateNode("Sally_W23_Success", @"Tulunganak! Saan ka a pulos agmaymaysa—awan ti rumbeng a panagbuteng!");

        UpdateNode("Sally_W24_Teach", @"Adda dagiti oras a kayatmo a dumawat iti tulong iti naurnos ken naayat a wagas.

Iti Ilokano, ti 'mabalin kadi a tulunganak?' ket kayatna a saoen ti 'can you help me?'

Nasayaat daytoy no kayatmo a dumawat iti tulong ti maysa a tao iti naurnos a wagas.

Denggem a naimbag: mabalin kadi a tulunganak?

Ita, padasem nga ibagam ti 'mabalin kadi a tulunganak?' iti bukodmo a timek.");
        UpdateNode("Sally_W24_Success", @"Mabalin kadi a tulunganak! Nasayaat unay!

Sapulem ni Tour Guide Lito para kadagiti sumaruno a kiddaw!");

        UpdateNode("Lito_W25_Teach", @"Agannadka! Adda kalesa nga lumabas!

No kasapulam a dumteng ti maysa a tao iti bassit a panaguray ken aguray kenka, mabalinmo nga ibaga ti 'urayennak.'

Kayatna a saoen ti 'please wait' wenno 'wait for me.'

Denggem a naimbag: urayennak.

Ita, padasem nga ibagam ti 'urayennak' iti bukodmo a timek.");
        UpdateNode("Lito_W25_Success", @"Urayennak! Umuna ti kinatalged kadagitoy a napipigsa a dalan!");

        UpdateNode("Lito_W26_Teach", @"No dumawatka iti maysa a tao a mangted iti maysa a banag kenka, mabalinmo nga agusar iti 'ikanmo man...'

Kayatna a saoen ti 'give me...' ken kalpasanna, iyungetmo ti banag a kayatmo.

Kas pagarigan: 'ikanmo man map'—'give me the map.'

Denggem a naimbag: ikanmo man map.

Ita, padasem nga ibagam ti 'ikanmo man' ken kalpasanna ti nagan ti banag a kayatmo.");
        UpdateNode("Lito_W26_Success", @"Ikanmo man! Naimbag ti panagaramidmo! Adda ditoy ti mapam iti Vigan!");

        UpdateNode("Lito_W27_Teach", @"Sakbay a damagem ti maysa a tao, naurnos a dumawat iti pammalubos.

Iti Ilokano, ti 'mabalin kadi agsaludsod?' ket kayatna a saoen ti 'can I ask?'

Mabalinmo nga agusar iti daytoy sakbay a dumawatka iti impormasyon iti maysa a tao.

Denggem a naimbag: mabalin kadi agsaludsod?

Ita, padasem nga ibagam ti 'mabalin kadi agsaludsod?' iti bukodmo a timek.");
        UpdateNode("Lito_W27_Success", @"Mabalin kadi agsaludsod! Sige, damagem laeng!

✨ NALUKATAN TI REQUESTS MILESTONE! ✨");

        UpdateNode("Lito_W28_Teach", @"Mapanka iti Plaza Salcedo ken saanmo a kayat ti agliko iti aniaman a dalan.

Iti Ilokano, ti 'agdiretso' ket kayatna a saoen ti 'go straight.'

Mabalinmo nga agusar iti daytoy no mangtedka iti direksyon wenno no ibagam iti maysa a tao nga agtultuloy a mapan iti sango.

Denggem a naimbag: agdiretso.

Ita, padasem nga ibagam ti 'agdiretso' iti bukodmo a timek.");
        UpdateNode("Lito_W28_Success", @"Agdiretso! Diretso laeng iti sango!

Mapanka ken Apo Lakay iti asideg ti bato a bubon para kadagiti sumaruno a direksyon!");

        UpdateNode("ApoLakay_W29_Teach", @"Tapno makadanonka iti lugar ti panagaramid iti Burnay, masapulmo a ibaga no ania a direksyon ti suroten.

Iti Ilokano, ti 'agliko iti kannigid' ket kayatna a saoen ti 'turn left.'

Usarem daytoy no ibagam iti maysa a tao nga agliko iti kannigid.

Denggem a naimbag: agliko iti kannigid.

Ita, padasem nga ibagam ti 'agliko iti kannigid' iti bukodmo a timek.");
        UpdateNode("ApoLakay_W29_Success", @"Agliko iti kannigid! Agliko iti kannigid—adda idiay dagiti pugon a paglutuan iti luto!");

        UpdateNode("ApoLakay_W30_Teach", @"No kayatmo a mapan ti maysa a tao iti sabali a direksyon, mabalinmo nga ibaga kenkuana nga agliko iti kannawan.

Iti Ilokano, ti 'agliko iti kannawan' ket kayatna a saoen ti 'turn right.'

Denggem a naimbag: agliko iti kannawan.

Ita, padasem nga ibagam ti 'agliko iti kannawan' iti bukodmo a timek.");
        UpdateNode("ApoLakay_W30_Success", @"Agliko iti kannawan! Agliko iti kannawan iti suli!");

        UpdateNode("ApoLakay_W31_Teach", @"Tapno makadanonka iti tuktok ti torre ken makitam ti probinsia iti baba, masapulmo nga ibaga iti maysa a tao nga umuli iti ngato.

Iti Ilokano, ti 'umuli iti ngato' ket kayatna a saoen ti 'go up.'

Usarem daytoy no mangidalan ka iti maysa a tao iti nangato a lugar.

Denggem a naimbag: umuli iti ngato.

Ita, padasem nga ibagam ti 'umuli iti ngato' iti bukodmo a timek.");
        UpdateNode("ApoLakay_W31_Success", @"Umuli iti ngato! Umuli ka iti tuktok!");

        UpdateNode("ApoLakay_W32_Teach", @"Kalpasan ti panagragsakmo iti makita a pasdek, masapulmo nga agsubli iti daga.

Iti Ilokano, ti 'bumaba' ket kayatna a saoen ti 'go down.'

Usarem daytoy no ibagam iti maysa a tao nga agturong iti nababbaba a lugar.

Denggem a naimbag: bumaba.

Ita, padasem nga ibagam ti 'bumaba' iti bukodmo a timek.");
        UpdateNode("ApoLakay_W32_Success", @"Bumaba! Agannadka iti addangmo!

Mapanka ken Tomas iti pottery yard!");

        UpdateNode("Tomas_W33_Teach", @"Naragsak nga isasangbayka iti clay yard!

No kayatmo a pagsardengen ti maysa a tao iti lugar a pakasasaadanna, mabalinmo nga ibaga ti 'agsardeng ditoy.'

Kayatna a saoen ti 'stop here.'

Denggem a naimbag: agsardeng ditoy.

Ita, padasem nga ibagam ti 'agsardeng ditoy' iti bukodmo a timek.");
        UpdateNode("Tomas_W33_Success", @"Agsardeng ditoy! Naimbag a lugar a pagsardengan!");

        UpdateNode("Tomas_W34_Teach", @"Adda dagiti oras a kasapulam a tawagan ti maysa a tao nga umay iti ayanmo.

Iti Ilokano, ti 'umay ditoy' ket kayatna a saoen ti 'come here.'

Usarem daytoy no kayatmo a dawaten iti maysa a tao nga umay iti ayanmo.

Denggem a naimbag: umay ditoy.

Ita, padasem nga ibagam ti 'umay ditoy' iti bukodmo a timek.");
        UpdateNode("Tomas_W34_Success", @"Umay ditoy! Umayka ken kitaem daytoy a pottery!");

        UpdateNode("Tomas_W35_Teach", @"No kayatmo a mapan ti maysa a tao iti sabali a lugar, mabalinmo nga idalan isuna a mapan iti sabali a lugar manipud iti ayanmo.

Iti Ilokano, ti 'mapan idiay' ket kayatna a saoen ti 'go there.'

Denggem a naimbag: mapan idiay.

Ita, padasem nga ibagam ti 'mapan idiay' iti bukodmo a timek.");
        UpdateNode("Tomas_W35_Success", @"Mapan idiay! Nasaysayaat ti nalamiis a lugar!");

        UpdateNode("Tomas_W36_Teach", @"No kayatmo a sumurot kenka ti maysa a tao, mabalinmo nga ibaga kenkuana nga sumurot.

Iti Ilokano, ti 'surotennak' ket kayatna a saoen ti 'follow me.'

Usarem daytoy no mangidalan ka iti maysa a tao iti sabali a lugar.

Denggem a naimbag: surotennak.

Ita, padasem nga ibagam ti 'surotennak' iti bukodmo a timek.");
        UpdateNode("Tomas_W36_Success", @"Surotennak! Sumurotka kaniak iti lugar a pagpabassitan dagiti banga!");

        UpdateNode("Klara_W37_Teach", @"No kasapulam a mapagtalinaed ti maysa a tao iti ayanna bayat nga adda aramidem, mabalinmo nga ibaga kenkuana nga aguray.

Iti Ilokano, ti 'uray ditoy' ket kayatna a saoen ti 'wait here.'

Denggem a naimbag: uray ditoy.

Ita, padasem nga ibagam ti 'uray ditoy' iti bukodmo a timek.");
        UpdateNode("Klara_W37_Success", @"Uray ditoy!

✨ NALUKATAN TI DIRECTIONS MILESTONE! ✨

Makisaritaka manen kaniak para kadagiti leksyon iti panagbilang!");

        UpdateNode("Klara_W38_Teach", @"Rugiantayo ti panagbilang iti Ilokano.

Ti sao a 'maysa' ket kayatna a saoen ti 'one.'

Usarem daytoy no agbilangka iti maysa laeng a tao wenno banag.

Kas pagarigan, no makitam ti maysa a kahon, mabalinmo nga ibaga ti 'maysa.'

Denggem a naimbag: maysa.

Ita, padasem nga ibagam ti 'maysa' iti bukodmo a timek.");
        UpdateNode("Klara_W38_Success", @"Maysa! Maysa laeng a napateg a kahon!");

        UpdateNode("Klara_W39_Teach", @"No agbilangka iti dua a banag, tattao, wenno aniaman a banag, agusar ti Ilokano iti 'dua.'

Kayatna a saoen ti 'two.'

Denggem a naimbag: dua.

Ita, padasem nga ibagam ti 'dua' iti bukodmo a timek.");
        UpdateNode("Klara_W39_Success", @"Dua! Dua a agpapada a tugaw!");

        UpdateNode("Klara_W40_Teach", @"Ti sumaruno a numero ket 'tallo.'

Iti Ilokano, ti 'tallo' ket kayatna a saoen ti 'three.'

Kas pagarigan, mabalinmo nga agusar iti daytoy no agbilangka iti tallo a banag.

Denggem a naimbag: tallo.

Ita, padasem nga ibagam ti 'tallo' iti bukodmo a timek.");
        UpdateNode("Klara_W40_Success", @"Tallo! Tallo a silaw ti lana!

Mapanka ken Tala, ti aglako iti bagnet, para iti sumaruno a numero!");

        UpdateNode("Tala_W41_Teach", @"Padayawantayo pay ti panagbilang.

Iti Ilokano, ti 'uppat' ket kayatna a saoen ti 'four.'

Mabalinmo nga agusar iti daytoy no agbilangka iti uppat a banag, kas iti uppat a piraso ti bagnet.

Denggem a naimbag: uppat.

Ita, padasem nga ibagam ti 'uppat' iti bukodmo a timek.");
        UpdateNode("Tala_W41_Success", @"Uppat! Uppat a napudot ken napudpudot a bagnet!");

        UpdateNode("Tala_W42_Teach", @"Ti sumaruno a numero ket 'lima.'

Iti Ilokano, ti 'lima' ket kayatna a saoen ti 'five.'

Denggem a naimbag: lima.

Ita, padasem nga ibagam ti 'lima' iti bukodmo a timek.");
        UpdateNode("Tala_W42_Success", @"Lima! Lima a kilo ti naimas a bagnet!");

        UpdateNode("Tala_W43_Teach", @"Iti Ilokano, ti numero a 'six' ket 'innem.'

Denggem a naimbag: innem.

Ita, padasem nga ibagam ti 'innem' iti bukodmo a timek.");
        UpdateNode("Tala_W43_Success", @"Innem! Innem a resipe ti pamilya!");

        UpdateNode("Tala_W44_Teach", @"Ti sao iti Ilokano para iti 'seven' ket 'pito.'

Denggem a naimbag: pito.

Ita, padasem nga ibagam ti 'pito' iti bukodmo a timek.");
        UpdateNode("Tala_W44_Success", @"Pito! Pito!

Daguska ken Mang Lance, ti agmaneho iti kalesa, tapno malpasmo ti panagbilang!");

        UpdateNode("MangLance_Intro", @"Whoa, uray bassit! Nagtalaw ti wheel pin ti kalesak!

Pangngaasim, sapulem ti wheel pin tapno natalged ti panagmaneho mi ni Barnaby iti kalesa!");

        UpdateNode("MangLance_W45_Teach", @"Agpuyat! Agyamanak iti panangurnosmo iti wheelko!

Itultuloytayo ti panagbilang.

Iti Ilokano, ti 'walo' ket kayatna a saoen ti 'eight.'

Denggem a naimbag: walo.

Ita, padasem nga ibagam ti 'walo' iti bukodmo a timek.");
        UpdateNode("MangLance_W45_Success", @"Walo! Walo a treats para ken Barnaby!");

        UpdateNode("MangLance_W46_Teach", @"Ti sumaruno a numero ket 'siam.'

Iti Ilokano, ti 'siam' ket kayatna a saoen ti 'nine.'

Denggem a naimbag: siam.

Ita, padasem nga ibagam ti 'siam' iti bukodmo a timek.");
        UpdateNode("MangLance_W46_Success", @"Siam! Siam! Asidegenmon ti pagpatinggaan ti panagbilang!");

        UpdateNode("MangLance_W47_Teach", @"Ket ita, nakadanontayon iti sangapulo!

Iti Ilokano, ti 'sangapulo' ket kayatna a saoen ti 'ten.'

Denggem a naimbag: sangapulo.

Ita, padasem nga ibagam ti 'sangapulo' iti bukodmo a timek.");
        UpdateNode("MangLance_W47_Success", @"Sangapulo! Nakadanonka iti sangapulo!

🏆 NALPAS TI LEVEL II: FUNCTIONAL & NAVIGATIONAL! 🏆

Nasursurom no kasano ti agkiddaw iti tulong, agturong kadagiti dalan, mangted iti direksyon, ken agusar kadagiti numero iti inaldaw-aldaw a kasasaad.

Ita, sursuruentayo no kasano a iladawan ti Ilokano dagiti aramid, tattao, ken kapanunotan.

Naragsak a panagsangbay iti Level III: Grammatical Foundations!");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=yellow>[Level 2] Updated 29 dialogue nodes with translations!</color>");
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
