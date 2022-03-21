import java.math.BigDecimal;
import java.math.BigInteger;
import java.time.Duration;
import java.time.Instant;
import java.util.Random;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.PrintWriter;

public class App {
    public static void main(String[] args) throws Exception {

        // BenchmarkDoubleToBigInteger();
        // System.in.read();
         //java.util.concurrent.TimeUnit.MINUTES.sleep(90);
        // Creating a BigInteger object
        BigInteger big;
        BigInteger squareRoot = BigInteger.ZERO;
        big = new BigInteger("0");                      if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + " and " + NewtonPlusSqrt(big) + "!!!!!!!!!!!!!"); }
        big = new BigInteger("1");                      if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + " and " + NewtonPlusSqrt(big) + "!!!!!!!!!!!!!"); }
        big = new BigInteger("2");                      if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + " and " + NewtonPlusSqrt(big) + "!!!!!!!!!!!!!"); }
        big = new BigInteger("4503599761588224");       if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + " and " + NewtonPlusSqrt(big) + "!!!!!!!!!!!!!"); }
        big = new BigInteger("144838757784765629");     if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + " and " + NewtonPlusSqrt(big) + "!!!!!!!!!!!!!"); }
        big = new BigInteger("4611686018427387904");    if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + " and " + NewtonPlusSqrt(big) + "!!!!!!!!!!!!!"); }
        big = new BigInteger("65785897164448191381343511924499180834109624990100754110346692714720833794182365156704520567066494452568598038317099669513516096681618601973599684423150328823149127367318079223120613816366038825604373268484954782110429835105286425333570541703714024151532084137071597682869259489221172755742364472724201391713876228852531646764843076854104581946061772267221757994736893926938160629380056553372248214368909504039212458266423980657106363733011856061121104369672147994640441778258160341955435109744447134256187215376894548013860157067815014854327054401293768815922507649024668392519266407522576716874831315048731693618952003216023606499644720147295275387516119527423680540664128864272031313852922193652901872732314717832642396584320613044874876038709312185088372090147112187657869779392688566837197222873583961493636492878943080433802748761360310302723190158716092892744929758873685478975276800670405063542783787175166169602615999466339596354504219501105673891354573209038920435930403019087822420187428656762642041998808872161199084947727133936559102364496735902940200177931856797143955996184320324843305825519761471929663872341396515529983727108459536090173169742307430044945248663781699303787795654384125310492475708658293822181665360");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " +  big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("568596673212406235539046204653574092779927528240990410308704492396460002250762210750361428976428226236231439839658527797601369407516351767038245884193317345297332639084030064798571562357325869523334241343457316070779167053230802243325508166810051970793029610805741205436326081505118688824250695502334609951573043497313485532249651668225178016840421469883558806604285808368323341479822807033581983205344039371458847573584264752900519079048915398981011181623230780288048890300634904517965949212204068737098107941104189698397409567698012505870983478073274795514339034169365773220230830720004807771185986855895045516625461415107861041712612909415282678933430993639015017973044506087648363018729847627791605903425315716867660373283634463025466898314552226041199227082594230096271404048950683874734785456093637644080259923926309749113053869920861252071824902816983919198943333352747567573205971722594392368757236981129345325357188283747755165211300361598909057331888013394644712040562578111312045020011010991816673666598318790155764029251076097495451756886293433937287357398346137053436703441717477280156524220517544784355270939618075800550360950261327034177704356076938995007627050124084857312674697721104308993667110188079684610462970532770811430407577836576516525347984663742735894854393178929653419127586692542189167843310656882269150136996004592038958466538491947533727965371911271931474363560773806290903082464818620463658436801310707825375068269725362589748265742091637498362621850566516960404061662197346676810108676564947944053523430298498377965136876382874888755098780874762932647407939626869067245075305802595524491063488862288004615122286564843300578758861996842723888825626998296253860396832728618268698513999105663450448455591453911622931238399649235353903164331408715648544484198462915062815114371906052899024506631206302172485246425734102246282006790515841269176368354750944457205740865919396101592164443131178363631180186481154637375276513183701970126748221898210963776797851581007115533834804644343822008468228860094391251339086238465678711921921284967851031385031907419916767858003766595842244869449534509496748434950293981618989570054257983701817992679002284807232891636649497294174834055117140618871480275671453660871756346174812413763546753450026532970380652053777318975717905464523846316531635812393163893169715944468523807676593337865609836993849998617745398537861679170415965793630853028066627328675239723557343598380919099547947117124600794798069455211709821342706038292764227379487605537218318247566029210526464224355667262491214506233857732726704711260425183953510241304266161977666456048204419815901425751876965458230836272990822362376876246567929545106942228163519038240884565231374383262437933921682301042259394323384010986971478111167473189572624136900260842462563632164438243406573308292515335842446029470620153455750269492648502995752698839327318101471308195682068310941060522835002396271185878639534383332814526736730627737675311440925963840167979468823970109167743381899347507787042067457538898702507081788658375961558876519917419284295948484882059236624597063874422807420548386494638142661819447373680561474726020600212494965888772592905717450857077427720367172542629011169439451164789672353614751341788973143362728269876015374623199859749885177199865505249137720400675981797711570054912715025084920361018230684483048532553044351189092760249689620742581712189291998116260173085751887604161409277198749738705418188423325214210791436123865095895924870449787198921748406270195538419589929583571270220608276936976047435099002481081568006269184926746075413902095833213112716082651523466102709809765278868521868323119552163151078268298543092909181764094354421443739939102266618363082870756251734285570963705511926675021687571952918187727056741057433095113623046731136780044772629021222742048526819043564191009305901298547848411807561630976907033779660614988066747079217915126796269532241434824559034294551034567089930561187776063042962098376001055021975735594097154610662494227608391831324251819834849972727806898988047156642000444156374099020509496337413749154812935620746943658043566102396889370920743841762392784731892754856936207021618371850662875222101060948251861698244863970080362619038241610264649305147250851826137983717857787292617945678440789654915586664827700527372511102728500979579398643014513213916697791050137689464063898748385088697075494961738641464073679536486848774189038880714063768855876078617505553790225908239315278348914126700695635803110855185780815614549031475503058796620205024302283909168380777135676289136283202774805599924261375761188156533898405212485642815583559044372013559333161812445651874731406291358975389500184031405897796170361475824855865416996816101180577179211466498528422630318977338028015876113173714172700603294777292007011893140268292139031620419978838233874410683183389592330610012298019178543866221491941389015906603598594523857857828887538768270002115444064670246189471105302854904357142640940815690815734212236064510656653497518955499351963564298916187123224379625736828158748021096975654285608875084594416617349440558333862887122384581582621428905712789260430593638211814462714113337065254852187384002227801421778675360047475402608367224990354216772160982391869523318958043157276234232215594952081263397029345447393215331410760682924198274455705944715191010466822858771476567511651447291833709417664994052378535962785039458646535298796251823002497493739453107285588189345055064989468830654731943635755096663118764896699043462398498063855211918516506112449276535417359689692816934359580186655249122981543576662853443920537700756370071176061687748929211550413374463270060822541705443889568085981718411771212910543081641827053808927056261093615521865229815551648410013951889654447956505928765294673621144189425512678052272847811478536361635041037247376079729955183288034878518775882046345262042085840029108259221965326603011451402215189442292326949370373918443853583166196123334746715570385322755860325668018143340905234499599661770553181685425109623262543554842563091462152389684151335425459512757268182388248548523806002139043631636292746232049543450881373125665610482569105547103522944485323651945010132405329295466906186502963615807071377352577596402482171032894689185345703931237197465495582572835620049008345615416894296528929647879711273709072418138224125107429443618724331758238114288955335876006203475145403711957920226577816782199167434890841283968630429187388429297294594396082808367568391279858533434198593052588130438526062765357196625443946092250933148070571533443620438382567098056574401350203053743912618224443564964963443981705183424089707049132420709512811684110954808225750117291699279190814324766521785636930646857218211339050239728030407504252402281110555437220027611206852489981972335663826111509206274876677013594436818577024753697807160957110918683574700041515759818141929214696536737555054156463211781351270996437552588723233229968280241742959860248850145435451592696399504097017400658966435059320799049397167451625262640117821825408096184107986323793568374539075737342874384546075573443054746055797583589009325216800125861872312825727671724348718906475191691504698280148131491140967107650139462912226865980464987962788562821048754369500679830978859725690019966684659376992815514045818430491026939697467871051042324292828993612736362860526073881154128878164128599611551530542310658302503782893657116944936194648228308865028057607312143533193693276156368189379813543149482685350792790900828570877584724011390068030679428412329640096515459755446277254206795751336660534427014290278841915158071763997616948849295574651034991146490119782815733533690996854686822010136733628846720978622539379847979030187244911605202840143861781982888178795784849924776497393293251973778927673416094397274685411386366555150802162781628659406215666823246623165293809466708313281347029191907077218303008657606493723248678087195932972884404760267321772756889788034941676516109268379862672702988397543495729736053680904777823723687628080981561711050363322368414855010665896948734225989419949098919172314701355903983867077527964924292955188146625777356737268809184825688808727316372675679905612856708689910533140809777396520507481327364392903800382176843167097065576641359796355206255921794399479538490606924139256862692724352340257725289913772940861331250854604953474112127163651856327913992832600123879966765945027720897535126324356794186885712704393607569538708105218104098849209423758863143894085086521731190291213196696379305734158272203101824107379076013035076712234971563844659606120406749074695967924573761014310011944566766342932466508424800571668641713486290921461220902791729023092771013548720715052229192246483610110598593104826970969087806572025329199569944106409193771552798814229945092476861583524722722759112066195996832270583763580479784998191784157314172850935658197344181379452332323193615353475333080892522126019108669481315395243573395324665363237446275972016556276331960182906303806899636009927546780543980346488212445140028771356388091942552442803773188008085351584825331255325030728733841409046107296730339245186862892760570481181616153002140782747878123422642967197887773195118140973221908721712441299887200866209083986770666824257088245624210982913142488708791987999144692110825246610047377251697461317512198538491122770376600660555409811367120173388378864060067155455090713541086467734664217720993609926519612776700477174101888188993458884880678252772213470425870780423917552701421891594456827968463820740935159076965202859200886098119890641044112881824831639132512603547695921402281940932405417994997938702814783404646751173368769430287371382741376186138486209145164636826028569305579088510904269435098378605169160732461767439018571424555136824347744527082607528180785762004430782108476943616681422686369572684280347505608117430283967292298696789525942300497917954715161971484652463862582725536721104119418568020996161818601764632840636654512368265608739854877931293276921822972877637476652414189723163275512609800132204020956071479807078747845492246682750495407588012902148137602445392176004165882375624372971093548185310117781416021878766399749320798339753778763002071834838306505750050805540210201136326020621556855005961134616936531575783426741040255892005909751600522657015249622142126076979990690329503176083139283356214159398656550117855920963884388077339193608835643128361137119861234131388516866300141373858812640966673370208976799490665174350806186760932933142007086682804757500330217466112164002695550502935300774079412928586340785624656954700151339024544642095334737169097624334788472340640760991267874048831593849530914907470681401965682331359610472406029801405712218159877957333208758173834527023282453958732806282317437672436010620079619244570903695496127956956370537136100022623247986844149235756854737785008277115265923928165200225322336346459556753143202165172227589300527336518245297791085001356912548092099238245360700904251961806593017791798456151653909623459064981589910213870726571579248087879927563066384322856888956287583958965883356240509710864472300769026240045989417656762302396004755336300292521981524065962669490439929857862990319598994639527337122261314730682010056463040758759229889469623898712489719894803269114736500294656194415411869308406146183676054911313131576490647259320992587418054623431837487101604749635114422516499029219293563309281873120759088798895107585724608028263483751331564960049361692704843923785240777313723141453380005341554361639106359775060438287533523856932311422822902626821353904546321484240526585044350171088733439015781558642861065686090670546148571755491286609432057060043971666742286929123667582156169592438262264874070917970547323393649441643765658590865329264618839034301964877180176616403723280116891858186445030814169336508840411566050522180360021655721892781045068966149022578480880921209467602646549684170791500075860710762219336741044349075645200291919578682567332551703896463617259078795321898551974712455647120157245246302781579972253305371481572681034639914639300323011324097949748477310566362105763912344548012887103371826153953590397376115194569981433472808913562082278719594578119855935945569601702547936472782015206840585788231913151015093662983988417437045652418696833683466810782825130439414165774425720297653926967414456360568582558597443506721014596982536734082869106415898445244563135907319363754872172732968447400156320191672297712593724792947325051014248915874773651986723659919087113447245598165982260966270996855626015566461178715461446100650588392938743410774620631872835529431975415586482202897851113697427399684025977623533827867513660088600035366734999866788911160860417088431720747173113244497590518629084473281861361283399287991613303988316391104279108152188690308248779942580210521290025393058453299589362878325391924052007947284168720206307284670229558925312818188165898373997681761608045055177236603993020863977914314891842571099458801905126172967955860909914918248496128402431735678905186029032850506351096008393724932390617611975528997095896345764537644314187545496811248642037001876864656020866004278021777761041348263458447185585845471066467853418222600902676527198074498128418084033054178895168187506127544181648175505145324458999126697899163798209934081");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("179769313486231570814527423731704356798070567525844996598917476803157260780028538760589558632766878171540458953514382464234321326889464182768467546703537516986049910576551282076245490090389328944075868508455133942304583236903222948165808559332123348274797826204144723168738177180919299881250404026184124858368");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("179769313486231570814527423731704356798070567525844996598917476803157260780028538760589558632766878171540458953514382464234321326889464182768467546703537516986049910576551282076245490090389328944075868508455133942304583236903222948165808559332123348274797826204144723168738177180919299881250404026184124858369");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("179769313486231570814527423731704356798070567525844996598917476803157260780028538760589558632766878171540458953514382464234321326889464182768467546703537516986050859197892009348423098673397576100060199749986929562362761314132383880020795324759350467498370284959417336104341480439649700322878491181830478233599");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("179769313486231570814527423731704356798070567525844996598917476803157260780028538760589558632766878171540458953514382464234321326889464182768467546703537516986050859197892009348423098673397576100060199749986929562362761314132383880020795324759350467498370284959417336104341480439649700322878491181830478233600");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " +  big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("17494584706016591027735461995965655369485392135483279753178087315506247479908101322432716538350151127201566210005644237814513392249452453615066147272228907663966390734640115862609428708808030883561312370933224354989584163634780158683901786449438459917087336832199985240528014645163631305415749573655211490978631716429164715576326122339425754435169992953750485069221610238394718337618921655783782041008005224393274487002390986157125495569904504979630450742020277163243700439394100971116982469820853805921150898151992772979321237326399758133");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("324869344822123891204500737190540217603582230298827943613070138634574543931529836644908557280814426421865640009546187334173413368040022188428404427615158419933534601057247685980219135338184905291081445059428169291870657858169275815840222956732487620761154654410650902413711236901782615917737119907905229234946211961080658388960638760959363844640743773892304002116832698921887645232477218304189719735593244966041503279433593532306881416962517923413587821230750081023226603959650598328121575017362314407084534778367861380310792005727284136781374900887396549343");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + "!!!!!!!!!!!!!"); }
        big = new BigInteger("197120777410685286861723419348662720446983624468633941814867274161329731855");
        if (!big.sqrt().equals(NewtonPlusSqrt(big))) {System.out.println("Fail on " + big.toString() + "!!!!!!!!!!!!!"); }


        if (true) { // true=Error checking, false= Benchmark
            ////////////////////////////////// Error Checking Area //////////////////////////////////
            // for (BigInteger temp = new BigInteger("4611686018427387904");
            //////////////////// /*temp.compareTo(BigInteger.ZERO) >0*/; temp =
            //////////////////// temp.add(BigInteger.valueOf(100007))) {
            int displayCounter = 0;
            BigInteger temp = BigDecimal.valueOf(4799901).toBigInteger();   // new BigInteger("9999901"); //BigDecimal.valueOf(1.7e37).toBigInteger();
                                                                           // //21267648410233606278185108989805867927
            while (true /* temp.compareTo(BigInteger.ZERO) >0 */) {

                NewtonPlusSqrt(temp);

                // BigInteger ans = temp.sqrt();
                // BigInteger sun = NewtonPlusSqrt(temp);
                // if (!ans.equals(sun)) {
                //     System.out.println(
                //             "Fail on " + temp.toString() + " Java: " + ans + " Sun: " + sun + " !!!!!!!!!!!!!");
                //     System.in.read();
                // }

                if ((displayCounter++ % 100000000) == 0) {
                    System.out.println("Status: " + temp.toString());
                }
                BigInteger addVal = temp.divide(BigInteger.valueOf(4799901)); // 101, 1009, 10007 , 100003, 1000003,
                                                                                // 9999901 , 179424673, 982451653
                // BigInteger addVal = BigInteger.ONE;
                temp = temp.add(addVal);
            }

        } else {
            //////////////////////////////////  Benchmarking Area  //////////////////////////////////
            try (PrintWriter writer = new PrintWriter("test.csv")) {
                StringBuilder sb = new StringBuilder();
                Random rand = new Random();
                Instant ends, starts = Instant.now();
                for (var bitsSize = 5; bitsSize < 200000; bitsSize += 1) {  //10 to (66 or 2500)

                    // Generate a random number
                    BigInteger bigInt = (new BigInteger(bitsSize, rand)).or(BigInteger.ONE.shiftLeft(bitsSize-1));
                    double timeWillTakeEAch = (0.133 * (double) bitsSize * (double) bitsSize) + (179.35 * (double) bitsSize) + 7693;
                    int loopCount = (int) ((20 * 1000000000.0) / timeWillTakeEAch);
                    
                    // if ulong or smaller increase test count
                    if (bitsSize < 65) 
                        loopCount *= 20;


                    ////////// Test Newton Plus //////////


                    System.gc ();
                    System.runFinalization ();
                    for (int i = 0; i < (loopCount>>7); i++) {
                        squareRoot = NewtonPlusSqrt(bigInt);
                    }
                    starts = Instant.now();
                    for (int i = 0; i < loopCount; i++) {
                        squareRoot = NewtonPlusSqrt(bigInt);        }    
                    ends = Instant.now();


                    sb.append(bitsSize + "," + loopCount + "," + Duration.between(starts,ends).dividedBy(loopCount/1000).toNanos() );
                    System.out.println("NewtonPlusSqrt - bits:" + bitsSize /* + big + " is " + squareRoot */ + " @ " + (Duration.between(starts, ends).dividedBy(loopCount/1000).toNanos() + " loops: " + loopCount));
   
                    System.gc ();
                    System.runFinalization ();
                    for (int i = 0; i < (loopCount>>7); i++) {
                        squareRoot = bigInt.sqrt();    }
                    starts = Instant.now();
                    for (int i = 0; i < loopCount; i++) {
                        squareRoot = bigInt.sqrt();             }
                    ends = Instant.now();

                    sb.append("," + Duration.between(starts, ends).dividedBy(loopCount/1000).toNanos() + "\r\n" );
                    System.out.println("BigIntegerSqrt - bits:" + bitsSize /*+ big + " is " + squareRoot*/ + " @ " + (Duration.between(starts, ends).dividedBy(loopCount/1000).toNanos() + " loops: " + loopCount + "\n"));

                    writer.write(sb.toString());
                    writer.flush();
                    sb.setLength(0);
                    
                }
            } catch (FileNotFoundException e) {
                System.out.println(e.getMessage());
            }
        }
    }


    // A fast square root by Ryan Scott White. 
    public static BigInteger NewtonPlusSqrt(BigInteger x) {
        if (x.compareTo(BigInteger.valueOf(144838757784765629L)) < 0) {
            long xAsLong = x.longValue();
            long vInt = (long)Math.sqrt(xAsLong);
            if (vInt * vInt > xAsLong)
                vInt--;
            return BigInteger.valueOf(vInt);  }

        double xAsDub = x.doubleValue();
        BigInteger val;
        if (xAsDub < 2.1267e37) // 2.12e37 largest here since sqrt(long.max*long.max) > long.max
        {
            long vInt = (long)Math.sqrt(xAsDub);
            val = BigInteger.valueOf((vInt + x.divide(BigInteger.valueOf(vInt)).longValue()) >> 1);
        }
        else if (xAsDub < 4.3322e127) {
            // Convert a double to a BigInteger
            long bits = Double.doubleToLongBits(Math.sqrt(xAsDub));
            int exp = ((int) (bits >> 52) & 0x7ff) - 1075;
            val = BigInteger.valueOf((bits & ((1L << 52)) - 1) | (1L << 52)).shiftLeft(exp);

            val = x.divide(val).add(val).shiftRight(1);
            if (xAsDub > 2e63) {
                val = x.divide(val).add(val).shiftRight(1);  }
        }
        else // handle large numbers over 4.3322e127
        {
            int xLen = x.bitLength();
            int wantedPrecision = ((xLen + 1) / 2);
            int xLenMod = xLen + (xLen & 1) + 1;

            //////// Do the first Sqrt on Hardware ////////
            long tempX = x.shiftRight(xLenMod - 63).longValue();
            double tempSqrt1 = Math.sqrt(tempX);
            long valLong = Double.doubleToLongBits(tempSqrt1) & 0x1fffffffffffffL;

            if (valLong == 0)
                valLong = 1L << 53;

            //////// Classic Newton Iterations ////////
            val = BigInteger.valueOf(valLong).shiftLeft(53 - 1)
                    .add((x.shiftRight(xLenMod - (3 * 53))).divide(BigInteger.valueOf(valLong)));

            int size = 106;
            for (; size < 256; size <<= 1) {
                val = val.shiftLeft(size - 1).add(x.shiftRight(xLenMod - (3 * size)).divide(val)); }

            if (xAsDub > 4e254) // 4e254 = 1<<845.77 
            {
                int numOfNewtonSteps = 31 - Integer.numberOfLeadingZeros(wantedPrecision / size) + 1;

                ////// Apply Starting Size ////////
                int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                int needToShiftBy = size - wantedSize;
                val = val.shiftRight(needToShiftBy);

                size = wantedSize;
                do {
                    //////// Newton Plus Iteration ////////
                    int shiftX = xLenMod - (3 * size);
                    BigInteger valSqrd = val.multiply(val).shiftLeft(size - 1);
                    BigInteger valSU = x.shiftRight(shiftX).subtract(valSqrd);
                    val = val.shiftLeft(size).add(valSU.divide(val));
                    size *= 2;
                } while (size < wantedPrecision);
            }
            val = val.shiftRight(size - wantedPrecision);
        }

        // Detect a round ups. This function can be further optimized - see article.
        // For a ~7% speed bump the following line can be removed but round-ups will occur.
        if (val.multiply(val).compareTo(x) > 0)
            val = val.subtract(BigInteger.ONE);

        // Enabling the below will guarantee an error is stopped for larger numbers.
        // Note: As of this writing, there are no known errors.
        BigInteger tmp = val.multiply(val);
        if (tmp.compareTo(x) > 0)  {
            System.out.println("val^2(" + val.multiply(val).toString() + ") >=  x(" + x.toString() + ")"); 
            System.console().readLine();
            //throw new Exception("Sqrt function had internal error - value too high");   
        }
        if (tmp.add(val.shiftLeft(1)).add(BigInteger.ONE).compareTo(x) <= 0) {
            System.out.println("(val+1)^2(" + val.add(BigInteger.ONE).multiply(val.add(BigInteger.ONE)).toString() + ") >=  x(" + x.toString() + ")"); 
            System.console().readLine();
            //throw new Exception("Sqrt function had internal error - value too low");    
        }

        return val;
    }


    private static void BenchmarkDoubleToBigInteger() {
        BigInteger m = BigInteger.ZERO;
        Instant starts, ends;
        double testVal = -9999999.99999999;

        starts = Instant.now();// long start = System.nanoTime();
        for (int i = 0; i < 1000000; i++) {

            m = DoubleToBigInteger(testVal);
        }
        ends = Instant.now();// long end = System.nanoTime();
        System.out.println("A " + m + " took " + (Duration.between(starts, ends).dividedBy(1000000)));

        starts = Instant.now();// long start = System.nanoTime();
        for (int i = 0; i < 1000000; i++) {

            m = BigDecimal.valueOf(testVal).toBigInteger();
        }
        ends = Instant.now();// long end = System.nanoTime();
        System.out.println("B " + m + " took " + (Duration.between(starts, ends).dividedBy(1000000)));
    }

    public static BigInteger bigIntSqRootFloor(BigInteger x)
            throws IllegalArgumentException {
        if (x.compareTo(BigInteger.ZERO) < 0) {
            throw new IllegalArgumentException("Negative argument.");
        }
        // square roots of 0 and 1 are trivial and
        // y == 0 will cause a divide-by-zero exception
        if (x.equals(BigInteger.ZERO) || x.equals(BigInteger.ONE)) {
            return x;
        } // end if
        BigInteger two = BigInteger.valueOf(2L);
        BigInteger y;
        // starting with y = x / 2 avoids magnitude issues with x squared
        for (y = x.divide(two); y.compareTo(x.divide(y)) > 0; y = ((x.divide(y)).add(y)).divide(two))
            ;
        return y;
    } // end bigIntSqRootFloor


    // Fast BigIntegerToDouble by Ryan Scott White 2/5/2022
    static BigInteger DoubleToBigInteger(double testVal) {
        long bits = Double.doubleToLongBits(testVal);
        int exp = ((int) (bits >> 52) & 0x7ff) - 1075;
        BigInteger m = BigInteger.valueOf((bits & ((1L << 52)) - 1) | (1L << 52)).shiftLeft(exp);
        return (bits >= 0) ? m : m.negate();
    }

}
