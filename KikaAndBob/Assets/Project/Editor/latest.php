<?php

$dir    = realpath( getcwd() . "/../" );
$files = array_diff(scandir($dir, 0), array('..', '.'));

$latestFile = "";
$latestTime = -1;

foreach( $files as $fileName )
{
	$file = $dir . "/" . $fileName;

	if( !is_dir($file) )
		continue;
	
	if( $fileName == "Latest" )
		continue;
	
	$stat = stat( $file );
	
	if( $stat["mtime"] > $time )
	{
		$time = $stat["mtime"]; 
		$latestFile = $file;
	}	
}

$basePath = str_replace("/Latest", "", dirname($_SERVER['REQUEST_URI']));

$redirectTo = "http://". $_SERVER['HTTP_HOST'] . $basePath . "/" . basename( $latestFile ) . "/";

header('Location: '. $redirectTo);
exit;

?>