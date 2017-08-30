<?php
Class b2blib {
var $lingua=1;
var $sep=';';
var $Descr=array();
var $GesQta=false;



function b2blib($dbhost="localhost",$dbuser="root",$dbpwd="root",$dbname="b2b",$lingua=1) {
	mysql_connect($dbhost, $dbuser, $dbpwd) OR DIE("Unable to connect to database");
	@mysql_select_db( "$dbname") or die( "Unable to select database");
	
    $this->lingua= $lingua;
    
	$this->Descr['Home'][1]='Home';
	$this->Descr['Home'][2]='Home';	
	$this->Descr['Ricerca'][1]='Ricerca';
	$this->Descr['Ricerca'][2]='Search';		
	$this->Descr['Carrello'][1]='Carrello';
	$this->Descr['Carrello'][2]='Cart';	
	$this->Descr['Ordini'][1]='Ordini';
	$this->Descr['Ordini'][2]='Invoices';	
	$this->Descr['Esci'][1]='Esci';
	$this->Descr['Esci'][2]='Logout';		
	$this->Descr['Benvenuto'][1]='Benvenuto';
	$this->Descr['Benvenuto'][2]='Welcome';		
	$this->Descr['Entra'][1]='Entra';
	$this->Descr['Entra'][2]='Go';	
	$this->Descr['Vai'][1]='Vai';
	$this->Descr['Vai'][2]='Go';
	$this->Descr['Ordina per'][1]='Ordina per';
	$this->Descr['Ordina per'][2]='Order by';	
	$this->Descr['Codice'][1]='Codice';
	$this->Descr['Codice'][2]='Code';	
	$this->Descr['Descrizione'][1]='Descr.';
	$this->Descr['Descrizione'][2]='Descr.';	
	$this->Descr['Note'][1]='Note';
	$this->Descr['Note'][2]='Note';	
	$this->Descr['Qta'][1]='Qta';
	$this->Descr['Qta'][2]='Qty';	
	$this->Descr['Prezzo'][1]='Prezzo';
	$this->Descr['Prezzo'][2]='Price';	
	$this->Descr['Sconto'][1]='Sconto';
	$this->Descr['Sconto'][2]='Disc.';		
	$this->Descr['Netto'][1]='Netto';
	$this->Descr['Netto'][2]='Net';		
	$this->Descr['Importo'][1]='Importo';
	$this->Descr['Importo'][2]='Value';	
	$this->Descr['ImportoIva'][1]='Totale';
	$this->Descr['ImportoIva'][2]='Total';		
	$this->Descr['Disponibile'][1]='Disponibile';
	$this->Descr['Disponibile'][2]='Available';		
	$this->Descr['Aggiungi al carrello'][1]='Aggiungi al carrello';
	$this->Descr['Aggiungi al carrello'][2]='Add to cart';		
	$this->Descr['Totale'][1]='Totale';
	$this->Descr['Totale'][2]='Total';	
	$this->Descr['Invia'][1]='Invia';
	$this->Descr['Invia'][2]='Send';		
	$this->Descr['ORDINE WEB'][1]='ORDINE WEB';
	$this->Descr['ORDINE WEB'][2]='WEB ORDER';	
	$this->Descr['Riepilogo ordine'][1]='Riepilogo ordine';
	$this->Descr['Riepilogo ordine'][2]='Order';	
	$this->Descr['Torna alla home'][1]='Torna alla home';
	$this->Descr['Torna alla home'][2]='Back to home';	
	$this->Descr['NoArt'][1]='Nessun articolo trovato!';
	$this->Descr['NoArt'][2]='Nothing found!';		
	$this->Descr['Iva'][1]='Iva';
	$this->Descr['Iva'][2]='Vat';	
	$this->Descr['DataDoc'][1]='Data';
	$this->Descr['DataDoc'][2]='Date';	
	$this->Descr['NumDoc'][1]='Num.';
	$this->Descr['NumDoc'][2]='Num.';	
	$this->Descr['QtaCons'][1]='Qta. Cons.';
	$this->Descr['QtaCons'][2]='Qty. Deliv.';	
	$this->Descr['Pagam'][1]='Pagamento';
	$this->Descr['Pagam'][2]='Payment';	
	$this->Descr['Destinaz'][1]='Destinazione';
	$this->Descr['Destinaz'][2]='Destination';		
	$this->Descr['SitOrd'][1]='Situazione ordini';
	$this->Descr['SitOrd'][2]='Orders status';			
	$this->Descr['Listino'][1]='Listino';
	$this->Descr['Listino'][2]='Price list';	
	$this->Descr["RagSoc"][1]='Ragione Sociale';
	$this->Descr["RagSoc"][2]='Customer Name';
	$this->Descr["Utenti"][1]='Utenti';
	$this->Descr["Utenti"][2]='Users';	
		
}

function checkusr($user,$pwd) {
	if ( trim($user)=='' || trim($pwd)=='' ) return false;

	$query="SELECT CodCli,CodDest,Password  FROM utenti WHERE User='$user'";
	$result = mysql_query($query);
	if ( mysql_num_rows($result) > 0 ) {
		$data=mysql_fetch_array($result);
		mysql_free_result($result);
		if (  trim($data['Password']) == $pwd ) {
            $sess_id= uniqid();
            $user_id= $data['CodCli'];
            $query= "INSERT INTO session VALUES(0,'$sess_id', now(), now(), '$user_id');";
            $result = mysql_query($query);
            
			return $sess_id;
		} else return false;
	} else  return false;
}

function checksess($sess_id) {
    if ( $sess_id=='') return false;

    $query= "DELETE FROM session WHERE hour(timediff(now(),data_ultimo_accesso))>0";
    $result = mysql_query($query);
    
    $query= "SELECT user_id FROM session WHERE session_id='$sess_id'";
    $result = mysql_query($query);
	if ( mysql_num_rows($result) > 0 ) {
        $data=mysql_fetch_array($result);
		mysql_free_result($result);
        $query= "UPDATE session SET data_ultimo_accesso=now() WHERE session_id='$sess_id'";
        $result = mysql_query($query);
        return $data['user_id'];
    }
    return false;
/*    
    $query= "SELECT user_id, hour(timediff(now(),data_ultimo_accesso)) as inattive FROM session WHERE session_id='$sess_id'";
    $result = mysql_query($query);
	if ( mysql_num_rows($result) > 0 ) {
        $data=mysql_fetch_array($result);
		mysql_free_result($result);
        if ( $data['inattive']==0 ) {
            $query= "UPDATE session SET data_ultimo_accesso=now() WHERE session_id='$sess_id'";
            $result = mysql_query($query);
            return $data['user_id'];
        } else {
            $query= "DELETE FROM session WHERE session_id='$sess_id'";
            $result = mysql_query($query);
        }
    }
    return false;
*/    
}

function benvenuto($user) {
    $query="SELECT * FROM clienti WHERE Cod=$user";
    $result = mysql_query($query);
    $data=mysql_fetch_array($result);
    mysql_free_result($result);
	
	return $data["RagSoc"];
}

function scorriliv($idc,&$arridliv,$liv,&$menutop) {
	$query="SELECT category.category_id,category.parent_id,category_description.name FROM category INNER JOIN category_description ON category.category_id=category_description.category_id AND category_description.language_id=".$this->lingua." WHERE category.category_id=$idc";
	$result=mysql_query($query);
	if ( $data=mysql_fetch_array($result) ) {	
		$arridliv[$liv]=$idc;
		$menutop=' - <a href="?op=articoli/cat&catid='.$data['category_id'].'&idl='.$this->lingua.'">'.$data['name'].'</a>'.$menutop;
		if ( $data['parent_id']>0 ) {
			$this->scorriliv($data['parent_id'],$arridliv,$liv+1,$menutop);
		}
	}
}

function categorie($cat_padre) {
	$query='SELECT category.category_id,category_description.name FROM ';
	$query.='category INNER JOIN category_description ON category_description.category_id=category.category_id ';
	$query.='WHERE category.parent_id='.$cat_padre.' AND category_description.language_id='.$this->lingua;
	$result=mysql_query($query);
    $categ= array();
	while ( $data=mysql_fetch_array($result) ) {
        $elem= array('category_id' => $data['category_id'], 'categoria' => $data['name']);
		array_push($categ,$elem);
	}
	mysql_free_result($result);	
    
    return $categ;
}

function categorie_all() {
	$query='SELECT category.parent_id,category.category_id,category_description.name FROM ';
	$query.='category INNER JOIN category_description ON category_description.category_id=category.category_id ';
	$query.='WHERE category_description.language_id='.$this->lingua;
    $query.=' ORDER BY category.parent_id, category_description.name';
    
	$result=mysql_query($query);
    $categ= array();
	while ( $data=mysql_fetch_array($result) ) {
        $elem= array('cat_padre' => $data['parent_id'],'category_id' => $data['category_id'], 'categoria' => $data['name']);
		array_push($categ,$elem);
	}
	mysql_free_result($result);	
    
    return $categ;
}

function articoli_all($user_id) {
	$query='SELECT product.category_id,product.product_id,product_description.name FROM ';
	$query.='product INNER JOIN product_description ON (product.product_id=product_description.product_id AND product_description.language_id='.$this->lingua.') ';
	$query.='INNER JOIN clienti ON clienti.cod='.$user_id.' INNER JOIN lisgen ON (lisgen.CodLis=clienti.CodLis AND lisgen.CodArt=product.model) ';
	$query.='LEFT JOIN liscla ON (liscla.CodClaSc=product.location) ';	
	$query.='LEFT JOIN liscli ON (liscli.CodCli=clienti.cod AND liscli.CodArt=product.location) ';    
    $query.='ORDER BY product.category_id,product_description.name';    

	$result=mysql_query($query);
    $eleprod= array();
	while ( $data=mysql_fetch_array($result) ) {
        $elem= array('category_id' => $data['category_id'], 'product_id' => $data['product_id'], 'nome' => $data['name']);
		array_push($eleprod,$elem);
	}
	mysql_free_result($result);	
    
    return $eleprod;    
}

function CalcolaSco ($ScoCli,$ScoCla,$ScoClCli) {

	$Sconto=$ScoCli;
	
	if ( $ScoCla!='' ) $Sconto=$ScoCla;
	
	if ( $ScoClCli!='' && $ScoClCli!=0 ) $Sconto=$ScoClCli;

	return $Sconto;
}


function articoli($user_id,$cat,$riclib,$ord=0) {

    $eleprod= array();    
	//Condizioni ricerca
    $where= '';
	if ( $cat>0 )  $where='WHERE product.category_id='.$cat;
	if ( $riclib!='' ) {
		$riclib=strtolower($riclib);
		$where.="WHERE ( LOWER(product.model) LIKE '%$riclib%' OR LOWER(product_description.name) LIKE '%$riclib%' OR LOWER(product_description.description) LIKE '%$riclib%' )";
	}

	//Elenco articoli
	$query='SELECT product.product_id,product.model,product.image,product_description.name,product_description.description,product.AlIva,IF(liscli.Prezzo>0,liscli.Prezzo,lisgen.Prezzo) as PrLordo,clienti.ScoPer1 as ScoCli,liscla.Sconto1 as ScoCla,liscli.Sconto1 as ScoClCli FROM ';
	$query.='product INNER JOIN product_description ON (product.product_id=product_description.product_id AND product_description.language_id='.$this->lingua.') ';
	$query.='INNER JOIN clienti ON clienti.cod='.$user_id.' INNER JOIN lisgen ON (lisgen.CodLis=clienti.CodLis AND lisgen.CodArt=product.model) ';
	$query.='LEFT JOIN liscla ON (liscla.CodClaSc=product.location) ';	
	$query.='LEFT JOIN liscli ON (liscli.CodCli=clienti.cod AND liscli.CodArt=product.location) ';
	$query.=$where;	
    
	switch ($ord) {
		case 0:
			$query.=' ORDER BY product_description.name ASC ';
			break;
		case 1:
			$query.=' ORDER BY product_description.name DESC ';			
			break;
		case 2:
			$query.=' ORDER BY PrLordo ASC ';
			break;
		case 3:
			$query.=' ORDER BY PrLordo DESC ';			
			break;
		case 4:
			$query.=' ORDER BY product.model ASC ';
			break;
		case 5:
			$query.=' ORDER BY product.model DESC ';			
			break;			
	}
	$result=mysql_query($query);

	$i=0;
	while ( $data=mysql_fetch_array($result) ) {
		// Calcolo Sconto ?????		
		$Sco1= $this->CalcolaSco($data['ScoCli'],$data['ScoCla'],$data['ScoClCli']);
		
        $elem= array('idp' => $data['product_id'], 'codice' => trim($data['model']),'nome' => trim($data['name']), 'prezzo_lordo' => $data['PrLordo'], 'sconto' => $Sco1, 'aliva' => $data['AlIva'], 'descrizione' => trim($data['description']));
		array_push($eleprod,$elem);
		$i++;
	}
	mysql_free_result($result);	
	
	return $eleprod;
}

function schedart($user_id,$idp,$sizeimg) {
	$scheda= array();
    
	$query='SELECT product.product_id,product.model,product.image,product_description.name,product_description.description,product.AlIva,IF(liscli.Prezzo>0,liscli.Prezzo,lisgen.Prezzo) as PrLordo,clienti.ScoPer1 as ScoCli,liscla.Sconto1 as ScoCla,liscli.Sconto1 as ScoClCli FROM ';
	$query.='product INNER JOIN product_description ON (product.product_id=product_description.product_id AND product_description.language_id='.$this->lingua.') ';
	$query.='INNER JOIN clienti ON clienti.cod='.$user_id.' INNER JOIN lisgen ON (lisgen.CodLis=clienti.CodLis AND lisgen.CodArt=product.model) ';
	$query.='LEFT JOIN liscla ON (liscla.CodClaSc=product.location) ';	
	$query.='LEFT JOIN liscli ON (liscli.CodCli=clienti.cod AND liscli.CodArt=product.location) ';
	$query.='WHERE product.product_id='.$idp;
	$result=mysql_query($query);
	if ( $data=mysql_fetch_array($result) ) {
		$Sco1= $this->CalcolaSco($data['ScoCli'],$data['ScoCla'],$data['ScoClCli']);
		
		$idp=$data['product_id'];

		if ( $data['image']!='' ) {
            $foto='../tmp/'.$sizeimg.'x'.$sizeimg.'_'.$data['image'];
			$size = getimagesize('../fotoprod/'.$data['image']);
            $sizeimg= intval($sizeimg);
			$newwidth = $size[0];
			$newheight = $size[1];							
			if ( $size[0]>$size[1] ) {
				if ( $size[0]>$sizeimg ) {
					$newwidth = $sizeimg;
					$newheight = $size[1]*($sizeimg/$size[0]);
				}
			} else {
				if ( $size[1]>$sizeimg ) {
					$newheight = $sizeimg;
					$newwidth = $size[0]*($sizeimg/$size[1]);				
				}
			}
			// Load
			$thumb = imagecreatetruecolor($newwidth, $newheight);
			$source = imagecreatefromjpeg('../fotoprod/'.$data['image']);
			// Resize
			imagecopyresized($thumb, $source, 0, 0, 0, 0, $newwidth, $newheight, $size[0], $size[1]);
			// Output
			imagejpeg($thumb,$foto);	
            
		} else $foto='image/no_image-250x250.jpg';		
		$scheda.='<tr><td><span><b>'.$this->Descr["Disponibile"][$this->lingua].':</b></td>';
		if ( $this->GesQta ) {
			if ( $data['quantity']>0 ) 
				$disponibile= $this->Descr["Si"][$this->lingua];
			else $disponibile='No';
		} else {
			$disponibile= $this->Descr["Si"][$this->lingua];
		}
        $scheda= array('idp' => $data['product_id'],'codice' => trim($data['model']),'nome' => trim($data['name']),'descrizione' => trim($data['description']), 'prezzo_lordo' => $data['PrLordo'], 'sconto' => $Sco1, 'aliva' => $data['AlIva'], 'foto' => $foto, 'foto_len' => filesize($foto),'disponibile' => $disponibile);
	}
	
	return $scheda;
}


function inviaord($id_sess, $user_id,$carrello,$note1="",$note2="",$stampante="",$destinaz=0) {
	$query="SELECT * FROM clienti WHERE Cod=$user_id";
	$result = mysql_query($query);
	$data=mysql_fetch_array($result);
	mysql_free_result($result);

    foreach ($carrello as $cart) {
        $query="INSERT INTO carrello (CodArt,Qta,note,Prezzo,Sconto1,CodCli) VALUES('".$cart["codart"]."',".$cart["qta"].",'".$cart['note']."',".$cart['prezzo'].",".$cart['sconto1'].",'$id_sess')";
        mysql_query($query);
    }
    
	// Genero nome file
	$tmpfname = tempnam ("tmp", "CA".$user_id."_");
	$tmpfname = basename($tmpfname,".tmp").".txt";
   
	// Scrivo ordine su tabella
	$query="INSERT INTO ordini VALUES(0,".date("Y").",'".date("Y-m-d")."','$user_id','".addslashes($data["RagSoc"])."','".addslashes($data["Via"])."','".$data["Cap"]."','".addslashes($data["Citta"])."',";
	$query.="'".$data["Prov"]."','','".$data["PIva"]."','$tmpfname','".addslashes($note1)."','".addslashes($note2)."','','','','','$destinaz','I')";
	mysql_query($query);

	// id nuovo ordine
	$idord=mysql_insert_id();
	$piva=$data["PIva"];
    
	$query = "SELECT carrello.idcar,carrello.CodArt,product_description.name,carrello.Qta,carrello.note,carrello.Prezzo,carrello.Sconto1,product.AlIva FROM carrello INNER JOIN product ON carrello.CodArt=product.model INNER JOIN product_description ON ( product.product_id=product_description.product_id AND product_description.language_id=1) ";	
	$query.= "WHERE carrello.CodCli='$id_sess' ORDER BY idcar ";
	$result = mysql_query($query);
	
	$i=1;
	while ( $data=mysql_fetch_array($result) ) {
		// Inserisce riga ordine
		$netto=$data['Prezzo']-($data['Prezzo']*$data['Sconto1']/100);
		$query="INSERT INTO righeordini VALUES($idord,".date("Y").",'".addslashes($data["CodArt"])."','".addslashes($data["name"])."',".$netto.",".$data['Prezzo'].",".$data['Sconto1'].",0,0,".$data["Qta"].",'".addslashes($data['note'])."','".$data['AlIva']."')";
		mysql_query($query);
		
		if ( $this->GesQta ) {
			$query="UPDATE product SET quantity=quantity-".$data["Qta"]." WHERE model='".$data['CodArt']."'";
			mysql_query($query);
		}

		$i++;
	}
	mysql_free_result($result);

	//Aggiorno stato per importazione in Gamma
	$query = "UPDATE ordini SET Stato='N' WHERE NumOrd=$idord AND Anno=".date("Y");
	mysql_query($query);	

    $this->conford($user_id,$idord,'Umbria Equitazione','Via Citernese, 112','06016 San Giustino (PG)');	
    
    
	$query = "DELETE FROM carrello WHERE CodCli='$id_sess'";
	mysql_query($query);	
    
	return $idord;    
}

function conford($user_id,$idord,$ind1='Umbria Equitazione',$ind2='Via Citernese, 112',$ind3='06016 San Giustino (PG)') {
    $query = "SELECT * FROM ordini WHERE NumOrd=$idord AND Anno=".date("Y");
    $result = mysql_query($query);
    $data=mysql_fetch_array($result);
    mysql_free_result($result);
	
	$table='<table class="ordine_testa" cellpadding="2" cellspacing="0" width="98%">
  <tr>
    <td width="100%" colspan="2" ><span style="font-size: 11pt"><b>'.$this->Descr["Riepilogo ordine"][$this->lingua].'</b></span></td>
  </tr>
  <tr>
    <td width="49%" valign="top"><b>'.$ind1.'</b>
        <p><b>'.$ind2.'<br>
        '.$ind3.'</b></p>
      <p>&nbsp;</td>
    <td width="51%" valign="top"><b>'.$data["RagSoc"].'</b>
        <p><b>'.$data["Via"].'<br>'.$data["CAP"]." ".$data["Citta"]." ".$data["Prov"].'<br>'.$data["PIva"].'
        </b></p>
    </td>
  </tr>
  <tr><td colspan="2"><br>'.date("d/m/Y").'<br><br></td></tr>
</table>';

    $note=$data["note"]."<br>".$data["note2"]."<br>".$data["note3"]."<br>".$data["note4"]."<br>".$data["note5"]."<br>".$data["note6"];
	$table.=' <table class="ordine_corpo" cellpadding="2" cellspacing="0" width="98%">
		<tr class="titolo">
		<td width="85" ><b>'.$this->Descr["Codice"][$this->lingua].'</b></td>
		<td width="210" ><b>'.$this->Descr["Descrizione"][$this->lingua].'
         / '.$this->Descr["Note"][$this->lingua].'</b></td>
		<td width="58" >
        <p align="right"><b>'.$this->Descr["Prezzo"][$this->lingua].'</b></p>
		</td>
		<td width="40" >
        <p align="right"><b>'.$this->Descr["Sconto"][$this->lingua].'</b></p>
		</td>		
		<td width="51" >
        <p align="right"><b>'.$this->Descr["Netto"][$this->lingua].'</b></p>
		</td>		
		<td width="26" ><b>'.$this->Descr["Qta"][$this->lingua].'</b></td>
		<td width="51" >
        <p align="right"><b>'.$this->Descr["Importo"][$this->lingua].'</b></p>
		</td>		
		<td width="40" >
        <p align="right"><b>'.$this->Descr["Iva"][$this->lingua].'</b></p>
		</td>
		<td width="51" >
        <p align="right"><b>'.$this->Descr["ImportoIva"][$this->lingua].'</b></p>
		</td>
		</tr>';
    $query = "SELECT * FROM righeordini WHERE IdOrd=$idord AND Anno=".date("Y");
    $result = mysql_query($query);

    $t2ot=0;
    while ( $data=mysql_fetch_array($result) ) {
		$importo=$data["Qta"]*$data["PzNetto"];
		$importoiva=$importo+($importo*$data["AlIva"]/100);
		$table.='<tr><td width="85">'.$data["CodArt"].'</td>
		<td width="210">'.$data["DesArt"].'<br><i>'.nl2br($data['note']).'</i></td>
		<td width="58">
        <p align="right">'.$this->formattaval($data["PzLordo"],2).'</p>
		</td>
		<td width="40">
        <p align="center">'.$this->formattaval($data["Sconto1"],0).'%</p>
		</td>		
		<td width="51">
        <p align="right">'.$this->formattaval($data["PzNetto"],2).'</p>
		</td>
		<td width="26"><p align="right">'.$data["Qta"].'</p></td>
		<td width="51">
        <p align="right">'.$this->formattaval($importo,2).'</p>
		</td>		
		<td width="40">
        <p align="right">'.trim($data["AlIva"]).'%</p>
		</td>		
		<td width="51">
        <p align="right">'.$this->formattaval($importoiva,2).'</p>
		</td>
		</tr>';	  
        $t2ot+=$importoiva;
    }
	$table.='<tr>
     <td  colspan="8" >
     <p align="right"><b>'.$this->Descr["Totale"][$this->lingua].'</b></td>
     <td width="51" >
     <p align="right"><b>'.$this->formattaval($t2ot,2).'</b></td>
	</tr>
    <tr>
      <td width="642" colspan="9">
        <b>'.$this->Descr["Note"][$this->lingua].'<br>
        </b>'.$note.'</td>
	</tr></table>';
	$mess=$table;
	
    mysql_free_result($result);

    if ( $user_id==0 ) {
        $data['Email']= 'ordini@umbriaequitazione.com, info@umbriaequitazione.com';
    } else {
        $query = "SELECT Email FROM clienti WHERE Cod=$user_id";
        $result = mysql_query($query);
        $data=mysql_fetch_array($result);
        mysql_free_result($result);
        
        $this->InviaEmail('luca.cimarossa@gmail.com',"Riepilogo ordine (mobile)",strip_tags($mess),$mess);
    }
    
	if ( $data['Email']!='' ) {
		$this->InviaEmail($data['Email'],"Riepilogo ordine",strip_tags($mess),$mess);
	}		

	return true;
}

function InviaEmail($ind_email,$oggetto,$mess_plain,$mess_html) {
    // costruiamo alcune intestazioni generali
     $header = "From: Umbria Equitazione <info@umbriaequitazione.com>\n";
    //	 $header .= "CC: Altro Ricevente <altroricevente@dominio.net>\n";
     $header .= "X-Mailer: Il nostro Php\n";

    // generiamo la stringa che funge da separatore
     $boundary = "==String_Boundary_x" .md5(time()). "x";

    // costruiamo le intestazioni che specificano
     // un messaggio costituito da pi??rti alternative
     $header .= "MIME-Version: 1.0\n";
     $header .= "Content-Type: multipart/alternative;\n";
     $header .= " boundary=\"$boundary\";\n\n";

    // questa parte del messaggio viene visualizzata
    // solo se il programma non sa interpretare
    // i MIME poich? posta prima della stringa boundary
     $messaggio = "Se visualizzi questo testo il tuo programma non supporta i MIME\n\n";

    // inizia la prima parte del messaggio in testo puro
     $messaggio .= "--$boundary\n";
     $messaggio .= "Content-Type: text/plain; charset=\"iso-8859-1\"\n";
     $messaggio .= "Content-Transfer-Encoding: 7bit\n\n";
     $messaggio .= $mess_plain.".\n\n";

    // inizia la seconda parte del messaggio in formato html
     $messaggio .= "--$boundary\n";
     $messaggio .= "Content-Type: text/html; charset=\"iso-8859-1\"\n";
     $messaggio .= "Content-Transfer-Encoding: 7bit\n\n";
     $messaggio .= "<html><body>".$mess_html."</body></html>\n";

    // chiusura del messaggio con la stringa boundary
     $messaggio .= "--$boundary--\n";

     $subject = $oggetto;
          
	if( !@mail($ind_email, $subject, $messaggio, $header) ) {
		echo "errore nell'invio dell'e-mail!";
		return false;
	}
	return true;
}

function sitord($user_id) {
    $eleord = array();
    
	$query='SELECT DataDoc,NumDoc,SUM(Qta) as QtaT,SUM(ImportoIva) as TotO FROM stato_ordini WHERE CodCli='.$user_id.' GROUP BY DataDoc,NumDOc ORDER BY DataDoc desc';
    $result = mysql_query($query);
    $data=mysql_fetch_array($result);

    while ( $data=mysql_fetch_array($result) ) {	
        $elem= array('DataDoc' => $data['DataDoc'], 'NumDoc' => trim($data['NumDoc']),'QtaT' => $data['QtaT'], 'TotO' => $this->formattaval($data['TotO'],2));
		array_push($eleord,$elem);        
    }
    mysql_free_result($result);	
	
    
	return $eleord;
}

function dettord($user_id,$datadoc,$numdoc) {
    $eleord = array();
    
	$query="SELECT * FROM stato_ordini WHERE CodCli=$user_id AND DataDoc='$datadoc' AND NumDoc=$numdoc";
    $result = mysql_query($query);
    $data=mysql_fetch_array($result);
	
    while ( $data=mysql_fetch_array($result) ) {	
        $elem= array('CodArt' => $data['CodArt'], 'DesArt' => $data['DesArt'],'Prezzo' => $this->formattaval($data['Prezzo'],2), 'Sconto1' => $this->formattaval($data['Sconto1'],0), 'Qta' => $this->formattaval($data['Qta'],0), 'Importo' => $this->formattaval($data['Importo'],2), 'AlIva' => trim($data['AlIva']), 'ImportoIva' => $this->formattaval($data['ImportoIva'],2));
		array_push($eleord,$elem);        
    }
    mysql_free_result($result);	

	return $eleord;
}

function elencofoto($cartella) {
    $elefoto = array();
    
    chdir($cartella);
    foreach (glob("*.jpg") as $filename) {
        $elem= array('filename' => $filename, 'size' => filesize($filename));
        array_push($elefoto,$elem);
    }    
    
    return $elefoto;
}

function formattadata($valore) {
    $como = substr($valore,8,2).'/'.substr($valore,5,2).'/'.substr($valore,2,2);
    return $como;
}

function formattaval($valore,$dec) {
    $como = number_format($valore,$dec,',','.');
    return $como;
}

}
?>
