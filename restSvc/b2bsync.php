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

$returnObject = (object) array( 'codice' => '99', 'descrizione' => 'nessun servizio trovato' );

/* Based on the method, use the arguments to figure out
   whether you're working with an individual or a collection, 
   then do your processing, and ultimately set $returnObject */
switch ($_SERVER['REQUEST_METHOD']) {
  case 'GET':  
    //FIXME: user_id preso dalla sessione  
    
    if ( $op == "ordini" ) {
        $eleord = array();
        
        $query = "SELECT ordini.NumOrd,ordini.Anno,ordini.Data,ordini.CodCli,ordini.CodDest,ordini.Note,ordini.note2,righeordini.CodArt,SUM(righeordini.Qta) as QtaT,righeordini.PzLordo,righeordini.Sconto1 ";
        $query.= "FROM ordini INNER JOIN righeordini ON ( ordini.NumOrd=righeordini.IdOrd AND ordini.Anno=righeordini.Anno) ";
        $query.= "WHERE ordini.Stato='N' GROUP BY ordini.NumOrd,ordini.Anno,ordini.Data,ordini.CodCli,ordini.CodDest,ordini.Note,ordini.note2,righeordini.CodArt,righeordini.PzLordo,righeordini.Sconto1";
        $result = mysql_query($query);
        $data=mysql_fetch_array($result);
        
        $Result = "<?xml version='1.0' encoding='utf-8'?>\n<ordini>\n";
        while ( $data=mysql_fetch_array($result) ) {	
            $Result .= " <ordine>\n";
            $elem= array('NumOrd' => $data['NumOrd'], 'Anno' => $data['Anno'],'Data' => $data['Data'], 'CodCli' => $data['CodCli'], 'CodDest' => $data['CodDest'], 'Note' => $data['Note'], 'note2' => $data['note2'], 'CodArt' => $data['CodArt'], 'QtaT' => $data['QtaT'], 'PzLordo' => $data['PzLordo'], 'Sconto1' => $data['Sconto1'] );
            foreach($elem as $key => $value) {
                $Result .=  "  <$key>$value</$key>\n";
            }
            $Result .= " </ordine>\n";        
        }
        mysql_free_result($result);	
        $Result .= "</ordini>\n";
        
        header('Content-Type: application/xml');
        echo $Result;
        exit();
        
        //$returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'ordini' => $eleord );
    }
    if ( $op == "elencofoto" ) {    
        $elenco_foto=$b2bl->elencofoto();
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok', 'elenco_foto' => $elenco_foto );            
    }        
    if ( $op == "conford" ) {    
        $ord = 0;
        if ( isset($_GET['ord']) ) $ord = $_GET['ord'];        
        $b2bl->conford(0,$ord);
        $returnObject = (object) array( 'codice' => '0', 'descrizione' => 'Ok');            
    }                
    break;    
    
  case 'PUT':       
    // Replace entire collection or member
    break;  
  case 'POST':   
    // Create new member
    break;
  case 'DELETE':    
    // Delete collection or member
    break;
}

echo json_encode($returnObject);
?>
