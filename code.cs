//Get all VMs
var vms = azure.VirtualMachines.List();

//Loop through VMs
foreach(IVirtualMachine vm in vms){                
    log.LogInformation("VM: " + vm.Name.ToString() + " - " + vm.PowerState.ToString());

    //Check if VM is running, if running check tags
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

        //Check what to do.
        if (poweroff){
            log.LogInformation("Deallocating VM: " + vm.Name.ToString());
            vm.DeallocateAsync(); //Don't wait.
        }
    }
} 
