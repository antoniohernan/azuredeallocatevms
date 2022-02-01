using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Compute.Fluent;
using System.Collections.Generic;

namespace my.functions
{
    public static class my_azure_function
    {
        [FunctionName("my_azure_function")]
        public static void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Powering off VMs without tag: powerstate=alwayson");

            var clientID = "bfcc403d-1f73–4ad4-xxxx-xxxxxxxxxxxx";
            var clientSecret = "SomethingDifficult";
            var tenantID = "12345678-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientID, clientSecret, tenantID, AzureEnvironment.AzureGlobalCloud);            
            var azure = Azure.Authenticate(credentials).WithDefaultSubscription();

           //Get all VMs
            var vms = azure.VirtualMachines.List();

            //Loop through VMs
            foreach(IVirtualMachine vm in vms){                
                log.LogInformation("VM: " + vm.Name.ToString() + " - " + vm.PowerState.ToString());
                
                //Check if VM is running
                if (vm.PowerState == PowerState.Running){
                    var poweroff = true;

                    //Loop through Tags when not none
                    foreach(KeyValuePair<string, string> tag in vm.Tags){
                        //If not tag - powerstate = alwayson -> deallocate VM
                        if(tag.Key.Equals("powerstate") && (tag.Value.Equals("alwayson"))){
                            log.LogInformation("VM: " + vm.Name.ToString() + " is set to 'always on'");
                            poweroff = false;
                            break; //No use to loop through other tags.
                        } 
                    } 

                    if (poweroff){
                        log.LogInformation("Deallocating VM: " + vm.Name.ToString());
                        vm.DeallocateAsync(); //Don't wait.
                    }
                }
            } 

            log.LogInformation("Done");
            return;
    
        }
    }
}
