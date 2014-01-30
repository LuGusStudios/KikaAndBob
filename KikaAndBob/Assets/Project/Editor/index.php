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

function human_filesize($bytes, $decimals = 2) {
  $sz = 'BKMGTP';
  $factor = floor((strlen($bytes) - 1) / 3);
  return sprintf("%.{$decimals}f", $bytes / pow(1024, $factor)) . @$sz[$factor];
}

$dir    = getcwd();
$files = array_diff(scandir($dir, 0), array('..', '.'));


echo "<h1>Kika And Bob webgames</h1>";
echo "<h2>". basename(realpath($dir)) ."</h2>"; 


foreach( $files as $file )
{
	if( !is_dir($file) )
		continue;
		
	$gameFiles = scandir($file); 
	
	$htmlFile = "";
	$size = "";
		
	echo "<table width=\"100%\">";
	foreach( $gameFiles as $gameFile )
	{
		
		if( strpos($gameFile, ".html") !== FALSE )
		{
			$htmlFile = $gameFile;
		}
		
		if( strpos($gameFile, ".unity3d") !== FALSE )
		{
			//print_r( $dir . "/" . $file . "/" . $gameFile );
			//print_r("\n");
			$size = filesize($dir . "/" . $file . "/" . $gameFile);
		}
		
		if( $htmlFile != "" && $size != "" )
		{
			echo '<tr><td><a target="_blank" href="'. $file . '/' . $htmlFile .'">' . $file  . '</a></td><td  width="50%">'. human_filesize($size) .'</td></tr>';
		}
	}	
	echo "</table>";
}

?>

	<h2>Extra</h2>
		<a href="http://lugus.be/KikaAndBob/AnimationSheets/1/Kika&Bob_AnimationTest.html">Animation Sheet 1</a><br/><br/>
		
		<a href="https://www.dropbox.com/s/78e4aa1yl2k0rwy/SceneMockups1.zip">Graphic Scene Mockups 1 (.zip)</a><br/>

		</div>

</body>
</html>