pipeline {
    agent any
    options {
        buildDiscarder(logRotator(numToKeepStr: "50"))
    }
    environment {
        MSBUILD = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin"
        AUTHOR_NAME = bat (
            script: "git show -s --format='%%an' HEAD",
            returnStdout: true
        ).split('\r\n')[2].trim()
    }
    stages {
        stage('Started Build') {
            steps {
                office365ConnectorSend color: '0000FF', message: "Latest status of build #${env.BUILD_ID}. Author: ${AUTHOR_NAME}", status: 'Started', webhookUrl: 'https://devblocknet.webhook.office.com/webhookb2/6b215103-1d46-4b68-b0fc-b4a8e43cbf8b@6b4bc897-d7e8-4d36-9d79-3fb5f24b50ad/JenkinsCI/d9f5278dc78c4c17b31b4b5cf29df72a/db0f7d76-8c74-4ddc-819a-0111e71f4e99/V2LlLkpZD2xsNT5xZj1miCIkwAGSyHGYNIR_YLXO_1llM1'
            }
        }
        stage('Build') {
            steps {
                bat label: '', script: '"D:\\JenkinBuild\\nuget.exe" restore "%WORKSPACE%/LinkIt.BubbleSheetPortal.Solution.sln"'
                bat "\"${MSBUILD}\\MSBuild.exe\" LinkIt.BubbleSheetPortal.Web/LinkIt.BubbleSheetPortal.Web.csproj /p:DeployOnBuild=true /p:PublishProfile=FolderProfile -t:rebuild"
                fileOperations([fileDeleteOperation(excludes: '', includes: 'LinkIt.BubbleSheetPortal.Web\\bin\\Release\\PublishOutput\\Web.config'), folderDeleteOperation('LinkIt.BubbleSheetPortal.Web\\bin\\Release\\PublishOutput\\PDFTool'), fileZipOperation(folderPath: 'LinkIt.BubbleSheetPortal.Web\\bin\\Release\\PublishOutput', outputFolderPath: '')])
            }
        }
        stage('Deploy') {
            stages {
                stage('Deploy internal') {
                    when {
                        branch 'internal/**'
                    }
                    steps {
                        ftpPublisher alwaysPublishFromMaster: false, continueOnError: false, failOnError: false, masterNodeName: '', paramPublish: null, publishers: [[configName: 'internal', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: "/Internal/Portal/${env.BRANCH_NAME.split("/")[1]}", remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false]]
                    }
                }
                stage('Deploy staging') {
                    when {
                        branch 'staging'
                    }
                    steps {
                        ftpPublisher alwaysPublishFromMaster: false, continueOnError: false, failOnError: false, masterNodeName: '', paramPublish: null, publishers: [[configName: 'us staging', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Staging/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false], [configName: 'au staging', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Staging/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false]]
                    }
                }
                stage('Deploy preprod') {
                    when {
                        branch 'release/**'
                    }
                    steps {
						script {
							def ftpServers = []
							if (env.PREPROD_HOST_NAME != null) {
								ftpServers = env.PREPROD_HOST_NAME.split(",")
								ftpServers = ftpServers.findAll { it.trim() != '' }
							}
							def remoteDir = 'Preprod/Portal'
							def localFile = 'PublishOutput.zip'
							
							if (ftpServers.size() > 0) {
								withCredentials([usernamePassword(credentialsId: 'e865f467-7fe6-49fb-86f1-e88d22ca844a', usernameVariable: 'ftpUsername', passwordVariable: 'ftpPassword')]) {
									ftpPassword = ftpPassword.replace('%', '%%')
									for (ftpServer in ftpServers) {
										echo "Start publishing file to ${ftpServer}"
										bat "curl -T ${localFile} ftp://%ftpUsername%:%ftpPassword%@${ftpServer}/${remoteDir}/${localFile}"
									}
								}
							} 
							else {
								ftpPublisher alwaysPublishFromMaster: false, continueOnError: false, failOnError: false, masterNodeName: '', paramPublish: null, publishers: [[configName: 'preprod', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Preprod/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false], [configName: 'preprod 2', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Preprod/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false]]
							}
						}
                    }
                }
                stage('Deploy US prod') {
                    when {
                        branch 'master'
                    }
                    steps {
						script {
							def ftpServers = []
							if (env.PRODUCTION_HOST_NAME != null) {
								ftpServers = env.PRODUCTION_HOST_NAME.split(",")
								ftpServers = ftpServers.findAll { it.trim() != '' }
							}
							def remoteDir = 'Production/Portal'
							def localFile = 'PublishOutput.zip'
							
							if (ftpServers.size() > 0) {
								withCredentials([usernamePassword(credentialsId: 'e865f467-7fe6-49fb-86f1-e88d22ca844a', usernameVariable: 'ftpUsername', passwordVariable: 'ftpPassword')]) {
									ftpPassword = ftpPassword.replace('%', '%%')
									for (ftpServer in ftpServers) {
										echo "Start publishing file to ${ftpServer}"
										bat "curl -T ${localFile} ftp://%ftpUsername%:%ftpPassword%@${ftpServer}/${remoteDir}/${localFile}"
									}
								}
							}
							else {
								ftpPublisher alwaysPublishFromMaster: false, continueOnError: false, failOnError: false, masterNodeName: '', paramPublish: null, publishers: [[configName: 'production 1', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Production/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false],[configName: 'production 2', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Production/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false]]
								archiveArtifacts artifacts: 'PublishOutput.zip', followSymlinks: false
							}
						}
                    }
                }
                stage('Deploy AU prod') {
                    when {
                        branch 'master_au'
                    }
                    steps {
                        ftpPublisher alwaysPublishFromMaster: false, continueOnError: false, failOnError: false, masterNodeName: '', paramPublish: null, publishers: [[configName: 'au production 1', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Production/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false],[configName: 'au production 2', transfers: [[asciiMode: false, cleanRemote: false, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '/Production/Portal', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'PublishOutput.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false]]
                        archiveArtifacts artifacts: 'PublishOutput.zip', followSymlinks: false
                    }
                }
            }
        }

    }
    post{
        success {
            office365ConnectorSend color: '00FF00', message: "Latest status of build #${env.BUILD_ID}. Author: ${AUTHOR_NAME}", status: 'Build Success', webhookUrl: 'https://devblocknet.webhook.office.com/webhookb2/6b215103-1d46-4b68-b0fc-b4a8e43cbf8b@6b4bc897-d7e8-4d36-9d79-3fb5f24b50ad/JenkinsCI/d9f5278dc78c4c17b31b4b5cf29df72a/db0f7d76-8c74-4ddc-819a-0111e71f4e99'
        }
        failure {
            office365ConnectorSend color: 'FF0000', message: "Latest status of build #${env.BUILD_ID}. Author: ${AUTHOR_NAME}", status: 'Build Failed', webhookUrl: 'https://devblocknet.webhook.office.com/webhookb2/6b215103-1d46-4b68-b0fc-b4a8e43cbf8b@6b4bc897-d7e8-4d36-9d79-3fb5f24b50ad/JenkinsCI/d9f5278dc78c4c17b31b4b5cf29df72a/db0f7d76-8c74-4ddc-819a-0111e71f4e99'
        }
		always  {
			dir("${WORKSPACE}@tmp") {
				deleteDir()
			}
			dir("${WORKSPACE}@script") {
				deleteDir()
			}
			
			dir("${WORKSPACE}@script@tmp") {
				deleteDir()
			}
		}
    }
}
