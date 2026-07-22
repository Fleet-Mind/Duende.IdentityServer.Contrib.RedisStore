node('windows-slave-fleetmind') {
  properties([disableConcurrentBuilds()])
  
  try {

    //-----------SHARED LIBRARIES------------------------------
    library identifier: "${env.DEFAULT_SHARED_LIBS}",
      retriever: modernSCM([$class: 'GitSCMSource',
        remote: "${env.DEFAULT_SHARED_LIBS_REPO}"
      ])

    pipelineProps.defaultBuildMultibranchProperties()

    //-----------GLOBAL VARIABLES------------------------------
    def version = "1.0.0. + ${env.BUILD_NUMBER}"
	def configType = "Release"
	def csProjFile = 'Duende.IdentityServer.Contrib.RedisStore\\Duende.IdentityServer.Contrib.RedisStore.csproj'
    def firstChar = env.BRANCH_NAME.replace("origin/", "").charAt(0)
    def pushPkgs = firstChar == '1' || firstChar == '2' || firstChar == '3' || firstChar == '4' || firstChar == '5' || firstChar == '6' || firstChar == '7' || firstChar == '8' || firstChar == '9'
	def buildNumber = "${env.BUILD_NUMBER}"

    //-----------STAGES------------------------------
    withEnv(["${env.MS_SONARQUBE_PATH}"]) { // Provides executable such as nuget and mstest

      stage('Clean Work Space') {
        cleanWs()
      }

      stage('Checkout') {
        timeout(10) {
          currentBuild.description = "Branch ${env.BRANCH_NAME}".replace("origin/", "")

          checkout([$class: 'GitSCM', branches: [
              [name: "${env.BRANCH_NAME}"]
            ],
            userRemoteConfigs: [
              [url: 'git@github.com:Fleet-Mind/Duende.IdentityServer.Contrib.RedisStore.git']
            ]
          ])
        }
      }
	  
		lock('global-build-counter') {
			stage('Generate Global Build Number') {
				buildNumber = fleetmindUtils.getGlobalBuildNumber()
			
				echo "Global Build Number: ${buildNumber}"
			}
		}

      stage('Set Build Number') {
		powershell ".\\set_release_version.ps1 ${buildNumber}"

		version = readFile('version.txt').trim()

        currentBuild.description = "${currentBuild.description} ${version}"
      }
	  
	  stage('Restore Packages') {
        timeout(20) {
            withCredentials([file(credentialsId: 'NuGetNexusSettings', variable: 'NuGet_Config')]) {
				bat "dotnet restore --configfile ${env.Nuget_config}"
            }
        }
      }
	  
		stage("Build") {
			bat "dotnet build -t:Clean"
			bat "dotnet build --configuration ${configType}"
		}
	  
		stage('Pack Package') {
			if (pushPkgs) {
				bat "dotnet pack --configuration ${configType} ${csProjFile}"
				bat "copy Duende.IdentityServer.Contrib.RedisStore\\bin\\Release\\*.nupkg ."
			}
		}
	  
		stage('Push Package') {
			if (pushPkgs) {
				withCredentials([string(credentialsId: 'FmNpmSettings', variable: 'gitHubToken')]) {
					bat "dotnet nuget push Duende.IdentityServer.Contrib.RedisStore.${version}.nupkg --api-key %gitHubToken% --source https://nuget.pkg.github.com/Fleet-Mind/index.json"
				}
			}
		}
    }
  } catch (e) {
    echo (e)
    currentBuild.result = 'FAILURE'
    throw e
  } finally {
    windowsNotify(currentBuild.result)
    cleanWs()
  }
  echo "RESULT: ${currentBuild.result}"
}

def windowsNotify(state) {
  node('master') {
    checkout([
      $class: 'GitSCM',
      branches: [
        [name: "${env.BRANCH_NAME}"]
      ],
      userRemoteConfigs: [
        [credentialsId: 'c052b16d-24f8-4515-8fc2-8a7232815952',
          url: 'git@github.com:Fleet-Mind/Duende.IdentityServer.Contrib.RedisStore.git'
        ]
      ]
    ])
  }
}
