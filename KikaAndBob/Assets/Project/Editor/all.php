<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">

<head>

<meta http-equiv="content-style-type" content="text/css" />
<meta http-equiv="content-script-type" content="text/javascript" />
<meta http-equiv="content-type" content="text/html;charset=iso-8859-1" />

<title>Kika And Bob</title>

<link rel="stylesheet" href="css.css" type="text/css" />
<script src="javascript.js" type="text/javascript"></script>

<style type="text/css">

html, body {

	background-color: #FFF;

	margin: 0;
	padding: 0;
	height: 100%;
	width: 100%;

	text-align: center;

	/*font-size: 12px;

	font-family: "Trebuchet MS", Verdana, Arial, Helvetica, sans-serif;*/

	color: #000;

}


#container {
	
	background-color: #FFF;

	/*border-left: 1px solid #666;
	border-right: 1px solid #666;*/

	position: relative;

	margin: 0 auto;

	width: 800px;

	text-align: center;

}

</style>

</head> 

<body>

	    <div id="container">

<?php

echo "<h1>Kika And Bob webgames</h1>";
echo "<h2>". basename(realpath($dir)) ."</h2>"; 


/*
function human_filesize($bytes, $decimals = 2) {
  $sz = 'BKMGTP';
  $factor = floor((strlen($bytes) - 1) / 3);
  return sprintf("%.{$decimals}f", $bytes / pow(1024, $factor)) . @$sz[$factor];
}
*/

$dir    = realpath( getcwd() . "/../" );
$files = array_diff(scandir($dir, 0), array('..', '.'));

$links = "";


foreach( $files as $fileName )
{
	$file = $dir . "/" . $fileName;

	if( !is_dir($file) )
		continue;
	
	if( $fileName == "Latest" )
		continue;
		
	if( $fileName == "All" )
		continue;
	
	$stat = stat( $file );
	
	$basePath = str_replace("/All", "", dirname($_SERVER['REQUEST_URI']));

	$link = "http://". $_SERVER['HTTP_HOST'] . $basePath . "/" . basename( $file ) . "/";
	
	$links = '<tr><td><a target="_blank" href="'. $link .'">' . $fileName  . '</a></td><td  width="50%">'. date("D d-m-Y H:i:s", $stat["mtime"]) .'</td></tr>' . $links;
	
	
	
	/*
	if( $stat["mtime"] > $time )
	{
		$time = $stat["mtime"]; 
		$latestFile = $file;
	}
	*/	
	
}


	
			
	echo "<table width=\"100%\">";

	echo $links;
	
	echo "</table>";

?>

		</div>

</body>
</html>