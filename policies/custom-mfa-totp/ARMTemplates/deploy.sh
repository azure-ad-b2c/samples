#!/bin/bash

function usage
{
    echo "usage: deploy "
	echo "[[[[-t | --TemplateFile] <File_Path>] "
	echo "  [[-p | --TemplateParameterFile] <File_Path>]"
	echo "  [[-r | --RepoUrl] <Url>]"
	echo "  [[-b | --Branch] <Branch_Name>]"
	echo " ] | "
	echo " [-h | --help]"
	echo "]"
}

# Main
TemplateFile='./Dev.json'
TemplateParameterFile='./paramcli.json'
RepoUrl='https://github.com/cljung/ToDoApp.git'
Branch='master'

while [ "$1" != "" ]; do
    case $1 in
        -t | --TemplateFile )           shift
                                TemplateFile=$1
                                ;;
        -p | --TemplateParameterFile )  shift
                                TemplateParameterFile=$1
                                ;;
        -r | --RepoUrl )                shift
                                RepoUrl=$1
                                ;;
        -b | --Branch )                 shift
                                Branch=$1
                                ;;
        -h | --help )           usage
                                exit
                                ;;
        * )                     usage
                                exit 1
    esac
    shift
done


echo "TemplateFile=${TemplateFile}"
echo "TemplateParameterFile=${TemplateParameterFile}"
echo "RepoUrl=${RepoUrl}"
echo "Branch=${Branch}"

START_TIME=$(date +”%r”)

if ! [ -f $TemplateFile ]
then
	echo -e "\033[1;31m template file $TemplateFile not found \033[0m"
elif ! [ -f $TemplateParameterFile ] 
then
	echo -e "\033[1;31m template parameter file $TemplateParameterFile not found \033[0m"
else 
	#Random
	#Used to randomize the names of the resources being created to avoid conflicts
	Unique=$RANDOM

	#Resource Group Properties
	RG_Name="QrCodeApp${Unique}-group"
	RG_Location="WestUS"

	#Read SQL Database credentials
	echo "Supply values for the following parameters:"
	read -p "sqlServerAdminLogin: " SqlUser
	read -s -p "sqlServerAdminPassword: " SqlPassword
	echo

	JSON=`cat $TemplateParameterFile`
	JSON=${JSON//\{UNIQUE\}/$Unique}
	JSON=${JSON//\{LOCATION\}/$RG_Location}
	JSON=${JSON//\{REPO\}/$RepoUrl}
	JSON=${JSON//\{BRANCH\}/$Branch}
	JSON=${JSON//\{USERNAME\}/$SqlUser}
	JSON=${JSON//\{PASSWORD\}/$SqlPassword}
	echo "$JSON" > "./temp.json"

	echo -e "\033[1;32m Creating Resource Group, App Service Plan, Web Apps and SQL Database... \033[0m"

	azure config mode arm

	azure group create --name $RG_Name --location $RG_Location --deployment-name $RG_Name --template-file $TemplateFile --parameters-file ./temp.json

	#Delete temporary JSON file as it stores SQL Database credentials
	rm ./temp.json

	echo -e "\033[1;32m ----------------------------------------- \033[0m"
	echo -e "\033[1;32m execution done \033[0m"
	echo -en "\007"
	
	END_TIME=$(date +”%r”)
	echo "Start= $START_TIME"
	echo "End= $END_TIME"
	read -p 'Press [Enter] key to continue...'
fi