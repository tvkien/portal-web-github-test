<?php

/*
	* define ENVIRONMENT - can be one of
	*
	* production
	* staging
	* development
	* developmentP  [used for testing against production database]
*/

define('AR_ENVIRONMENT', 'staging');

if (defined('AR_ENVIRONMENT'))
{
	switch (AR_ENVIRONMENT)
	{
		case 'development':
			//$appPath = "//ec2-174-129-215-222.compute-1.amazonaws.com/Remote1/LinkIt/phpapps/linkit_ar_dev/main.php";
			$appPath = "F:/LinkIt/phpapps/linkit_ar_dev/main.php";
			break;
		break;
	
		case 'developmentP':
			$appPath = "//ec2-174-129-208-179.compute-1.amazonaws.com/Remote1/LinkIt/phpapps/linkit_ar_devP/main.php";
			break;
			
		case 'staging':
			//$appPath = "//ec2-174-129-215-222.compute-1.amazonaws.com/Remote1/LinkIt/phpapps/linkit_ar_stage/main.php";
			$appPath = "F:/LinkIt/phpapps/linkit_ar_stage/main.php";
			break;
		
		case 'production':
			$appPath = "//ec2-174-129-208-179.compute-1.amazonaws.com/Remote1/LinkIt/phpapps/linkit_ar/main.php";
			break;
		break;

		default:
			exit('The application environment is not set correctly.');
	}
}
else
{
	exit('The application environment is not set correctly.');
}


require_once( $appPath );


?>