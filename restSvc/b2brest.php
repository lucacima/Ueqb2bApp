<?php
include("b2blibrest.php");

// Flag per errore se esecuzione > 30s
ini_set('max_execution_time', '0');
       
$op = '';
if ( isset($_GET['op']) ) $op = $_GET['op'];
$lingua=1;
if ( isset($_GET['lingua']) ) $lingua = $_GET['lingua'];

// Parametri MySQL
$hostname = "localhost";
$username = "si2bxkky_luca";
$password = "umbriaeq_2017";
$dbName   = "si2bxkky_umbriaeq";
//Classe b2blib
$b2bl = new b2blib($hostname, $username, $password,$dbName,$lingua);

header('Content-Type: application/json');

if ( $op!='' && $op!='login' ) {
    $session_id='';
    if ( isset($_GET['session_id']) ) $session_id = $_GET['session_id'];
    $user_id= $b2bl->checksess($session_id); 
    if ( !$user_id ) {
        $returnObject = (object) array( 'codice' => '1', 'descrizione' => 'sessione non valida, eseguire login', 'user_id' => '' );
        echo json_encode($returnObject);
        exit();
    }
}

//$apiArgArray = explode("/", substr(@$_SERVER['PATH_INFO'], 1));
$returnObject = (object) array( 'codice' => '99', 'descrizione' => 'nessun servizio trovato' );

/* Based on the method, use the arguments to figure out
   whether you're working with an individual or a collection, 
   then do your processing, and ultimately set $returnObject */
switch ($_SERVER['REQUEST_METHOD']) {
  case 'GET':  
    if ( $op == "infocli" ) {
        $ragsoc= $b2bl->benvenuto($user_id);
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'ragsoc' => $ragsoc );              
    }
    if ( $op == "categorie" ) {
        $cat_padre = 0;
        if ( isset($_GET['cat_padre']) ) $cat_padre = $_GET['cat_padre'];                
        $categorie= $b2bl->categorie($cat_padre);
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'categorie' => $categorie );            
    }
    if ( $op == "catall" ) {
        $cat_padre = 0;             
        $categorie= $b2bl->categorie_all();
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'categorie' => $categorie );            
    }    
    if ( $op == "articoli/cat" ) {    
        $ord = 0;
        if ( isset($_GET['ord']) ) $ord = $_GET['ord'];    
        $keyword = '';
        if ( isset($_GET['keyword']) ) $keyword = $_GET['keyword'];    

        $articoli=$b2bl->articoli($user_id,$_GET['cat_id'],$keyword,$ord);    
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'articoli' => $articoli );            
    }    
    if ( $op == "articoli/all" ) {    
        $articoli=$b2bl->articoli_all($user_id);    
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'articoli' => $articoli );            
    }    
    if ( $op == "schedart" ) {    
        $ord = 0;
        $size = 500;
        if ( isset($_GET['size']) ) $size = $_GET['size'];    
        $schart=$b2bl->schedart($user_id,$_GET['idp'],$size);
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'articolo' => $schart );            
    }        
    if ( $op == "sitord" ) {    
        $ordini=$b2bl->sitord($user_id);
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'ordini' => $ordini );            
    }        
    if ( $op == "dettord" ) {    
        $righe_ordine=$b2bl->dettord($user_id,$_GET['datadoc'],$_GET['numdoc']);
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'righe_ordine' => $righe_ordine );            
    }            
    break;    
    
  case 'PUT':       
    // Replace entire collection or member
    break;  
  case 'POST':   
    $inputJSON = file_get_contents('php://input');
    $valori= json_decode( $inputJSON, TRUE );  
    if ( $op == "login" ) {
        $user_id= $b2bl->checkusr($valori['username'], $valori['password']);
        if ( $user_id ) {
            $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'user_id' => $user_id );
        } else {            
            $returnObject = (object) array( 'codice' => '1', 'descrizione' => 'Username o password non validi', 'user_id' => '' );
        }
    }
    if ( $op == "inviaord" ) {
        $note="";
        if ( isset($_GET['note']) ) $note = $_GET['note'];  
        $ord_id= $b2bl->inviaord($session_id, $user_id, $valori, $note);
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'ord_id' => $ord_id );
    }
    break;
    
  case 'DELETE':    
    // Delete collection or member
    break;
}

echo json_encode($returnObject);


/*
function Visualizza($categ,$corpo,$menu) {
	$t = new Template(".", "keep");
	$t->set_file(array("b2b" => "templates/main.htm"));

	$t->set_var("categ",$categ);
	$t->set_var("corpo",$corpo);
	$t->set_var("menu",$menu);
	$t->parse("OUT", "b2b");
	$t->p("OUT");
}

if ( isset($_GET['lingua']) {
    set_session("lingua",$_GET['lingua']);
}

$op = '';
if ( isset($_GET['op']) ) $op = $_GET['op'];

$lingua = get_session("lingua");
if ( $lingua=="" ) $lingua=1;

//Classe b2blib
$b2bl = new b2blib($hostname, $username, $password,$dbName);



if ( $op == "" ) {
	$corpo=$b2bl->loginform();	
	Visualizza("&nbsp;",$corpo,"&nbsp;");
	exit();
}


if ( $op == "login" ) {
	if ( !isset($_POST['user']) && !isset($_POST['password']) ) {
		header("location: index.htm");
		exit();	
	}
	
	if ( strtoupper($_POST['user'])== "ADMIN" ) {
		$_POST['user']=strtoupper($_POST['user']);
		if ( $b2bl->checkadm( $_POST['user'],$_POST['password']) ) {
			$menu=$b2bl->menu(0,0);
			$categorie=$b2bl->clientiaz();
			$corpo=$b2bl->elecli('','',$_POST['user']);
			Visualizza($categorie,$corpo,$menu);
			exit();		
		}
	}
	if ( strtoupper($_POST['user'])== "AGEN" ) {
		$_POST['user']=strtoupper($_POST['user']);
		if ( $b2bl->checkadm($_POST['user'],$_POST['password']) ) {
			$menu=$b2bl->menu(0,0);
			$categorie=$b2bl->clientiaz();
			$corpo=$b2bl->elecli('','',$_POST['user']);
			Visualizza($categorie,$corpo,$menu);
			exit();		
		}
	}	
	
	if ( !$userid=$b2bl->checkusr($_POST['user'],$_POST['password']) ) {
		$corpo= "<br><center>utente o password errati!</center>";
		header("location: index.htm");
		exit();	
	} else {
		$user_dest = get_session("user_dest");
		$menu=$b2bl->menu($userid,$user_dest);
		$categorie=$b2bl->categorie(0,'Home');
		$corpo=$b2bl->articoli($userid,0,'');
		Visualizza($categorie,$corpo,$menu);
		exit();
	}
}

if ( $op == "articoli/cat" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");
		exit();	
	}
	$user_dest = get_session("user_dest");
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Home');
	if ( isset($_POST['keyword']) ) $keyword= $_POST['keyword'];
	if ( isset($_GET['keyword']) ) $keyword= $_GET['keyword'];
	if ( isset($_POST['ord']) ) $ord= $_POST['ord'];
	if ( isset($_GET['ord']) ) $ord= $_GET['ord'];
	
	$corpo=$b2bl->articoli($user_id,$_GET['catid'],$keyword,$_GET['pag'],$ord);
	Visualizza($categorie,$corpo,$menu);
	exit();	
}

if ( $op == "articoli/riclib" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");
		exit();	
	}
	$user_dest = get_session("user_dest");
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Ricerca');
	if ( isset($_POST['keyword']) ) $keyword= $_POST['keyword'];
	if ( isset($_GET['keyword']) ) $keyword= $_GET['keyword'];
	if ( $_POST['code']!='' ) 
		$corpo=$b2bl->schedart($user_id,0,$_GET['catid'],$_POST['code']);
	else $corpo=$b2bl->articoli($user_id,$_GET['catid'],$keyword,1,$_GET['pag'],$_POST['ord']);
	Visualizza($categorie,$corpo,$menu);
	exit();	
}

if ( $op == "schart" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");
		exit();	
	}
	$user_dest = get_session("user_dest");
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Home');
	$corpo=$b2bl->schedart($user_id,$_GET['prodid'],$_GET['catid']);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "aggcar" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");
		exit();	
	}
	$user_dest = get_session("user_dest");
	
	//Aggiungi al carrello
	$errore=$b2bl->aggcar($user_id,$user_dest,$_POST['codart'],$_POST['qty'],$_POST['note'],$_POST['prezzo'],$_POST['sconto1']);

	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Home');
	$corpo=$b2bl->schedart($user_id,$_GET['prodid'],$_GET['catid'],'',$errore);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "carrello" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");
		exit();	
	}
	$user_dest = get_session("user_dest");
	
    $errore="";
    if ( $_GET['op2'] == "modnote" ) {
        while (list($k,$v) = each($_POST['note'])) {
			$b2bl->modnotecar($user_id,$k,$v);
        }
    }
    if ( $_GET['op2'] == "modqta" ) {
        while (list($k,$v) = each($_POST['QtaO'])) {
			$errore=$b2bl->modqtycar($user_id,$k,$v);
        }
    }
    if ( $_GET['op2'] == "elimina" ) {
		$corpo=$b2bl->elicar($user_id,$_GET['codart']);
    }

	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Carrello');
	$corpo=$b2bl->carrello($user_id,$user_dest,$errore);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "ordina" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");		
		exit();	
	}
	$user_dest = get_session("user_dest");
	
	$ordid=$b2bl->inviaord($user_id,$user_dest,$_POST['note1'],$_POST['note2'],'PDFCreator',$_POST['destinaz']);
	
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Home');
	$corpo=$b2bl->conford($user_id,$ordid,'Umbria Equitazione','Via Citernese, 112','06016 San Giustino (PG)');
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "sitordini" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$user_dest = get_session("user_dest");
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Ordini');
	$corpo=$b2bl->sitord($user_id);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "dettordini" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$user_dest = get_session("user_dest");
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Ordini');
	$corpo=$b2bl->dettord($user_id,$_GET['datadoc'],$_GET['numdoc']);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "listino" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$filename=$b2bl->listino($user_id);
	header("Content-type: application/octet-stream");
	header("Content-Disposition: attachment; filename=\"listino.csv\"");

	$file_name="listini/".$filename;
	readfile($file_name);	
}

if ( $op == "profilo" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$user_dest = get_session("user_dest");
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Profilo');
	$corpo=$b2bl->profilo($user_id,$user_dest);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}
if ( $op == "aggprofilo" ) {
	$user_id = get_session("user_id");
	if( $user_id == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$user_dest = get_session("user_dest");
	$menu=$b2bl->menu($user_id,$user_dest);
	$categorie=$b2bl->categorie($_GET['catid'],'Profilo');
	$errore=$b2bl->aggprofilo($user_id,$user_dest,$_POST['User'],$_POST['Password']);
	$corpo=$b2bl->profilo($user_id,$user_dest,$errore);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "elecli" ) {
	$admin = get_session("admin");
	if( $admin == "" ) {
		header("location: index.htm");	
		exit();	
	}
	
	$menu=$b2bl->menu(0,0);
	$categorie=$b2bl->clientiaz();
	$corpo=$b2bl->elecli($_GET['lettera'],$_POST['ragsoc'],$admin);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "selcli" ) {
	$admin = get_session("admin");
	if( $admin == "" ) {
		header("location: index.htm");	
		exit();	
	}
	
	if ( !$userid=$b2bl->selcli($_POST['codcli'],$_POST['coddest']) ) {
		$corpo= "<br><center>utente o password errati!</center>";
		header("location: index.htm");
		exit();	
	} else {
		$user_dest = get_session("user_dest");
		$menu=$b2bl->menu($userid,$user_dest);
		$categorie=$b2bl->categorie(0,'Home');
		$corpo=$b2bl->articoli($userid,0,'');
		Visualizza($categorie,$corpo,$menu);
		exit();
	}
}

if ( $op == "aggcli" ) {
	$admin = get_session("admin");
	if( $admin == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$b2bl->aggcli($_POST['codcli'],$_POST['coddest'],$_POST['usr'],$_POST['pwd'],$_POST['InvEmail']!='' ? true : false);
	$menu=$b2bl->menu(0,0);
	$categorie=$b2bl->clientiaz();
	$corpo=$b2bl->elecli($_GET['lettera'],$_GET['ragsoc'],$admin);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "eleord" ) {
	$admin = get_session("admin");
	if( $admin == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$menu=$b2bl->menu(0,0);
	$categorie=$b2bl->clientiaz();
	$corpo=$b2bl->eleord($_POST['datada'],$_POST['dataa']);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}

if ( $op == "visord" ) {
	$admin = get_session("admin");
	if( $admin == "" ) {
		header("location: index.htm");	
		exit();	
	}
	$menu=$b2bl->menu(0,0);
	$categorie=$b2bl->clientiaz();
	$corpo=$b2bl->visord($_GET['idord']);
	Visualizza($categorie,$corpo,$menu);
	exit();		
}


if ( $op == "esci" ) {
    set_session("user_id",'');
	set_session("admin",'');
	header("location: index.htm");
		
	exit();	
}
*/
?>
