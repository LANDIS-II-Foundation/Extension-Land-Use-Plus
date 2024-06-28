import os
import threading
import time
#from osgeo import gdal


#Function to check for lockfile
def check_file_async(path, timer):
    while(True):
        if not os.path.isfile(path):
            print("file not found")
        else:
            print("lockfile found")
            return
        time.sleep(timer)

# ENTRY POINT - LANDIS executes pythom module starting here
        
#Start checking for lockfile
thr = threading.Thread(target = check_file_async, args=("lockfile", 1), kwargs={})
thr.start()
#Control will not flow past this point until the lockfile is found 
thr.join()

#We save the timestep in the lockfile itself for convenience
lockfile = open("lockfile", "r")
timestep = lockfile.read().strip()
print("Timestep: " + timestep) 

## Use timestep from LANDIS and output path to load an insect map
print("Reading insect output...")
output_path = "Insects/GypsyMoth-"+timestep+".img"
print("Output path to insect file:")
print(output_path)

## Check output has been written to system

#thr = threading.Thread(target = check_file_async, args=(raster_path, 1), kwargs={})
#thr.start()
#thr.join()
lockfile.close() #Close the lockfile stream before deletion
os.remove("lockfile") #delete the lock file and terminate. 
