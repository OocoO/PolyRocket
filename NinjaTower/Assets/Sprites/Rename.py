import os
import shutil

# replace all "//" to '#' mark

if __name__ == "__main__":
    # // 1. Get the sub folders in NinjaBird folder
    subFolders = os.listdir("NinjaBird")
    # // 2. Loop through the sub subFolders
    # // get current work directory
    cwd = os.getcwd()
    cwd = os.path.join(cwd, "NinjaBird")
    for subFolder in subFolders:
        
        # // 3. Get the sub sub folder path
        layerFolderPath = os.path.join(cwd, subFolder, "layers")
        fileDir = os.path.join(cwd, subFolder)

        if not os.path.isdir(fileDir):
            continue

        # // delete the layerFolderPath folder recursevly
        if os.path.exists(layerFolderPath):
            shutil.rmtree(layerFolderPath)
            os.remove(layerFolderPath + ".meta")

        # // 4. Get all files in fileDir
        # files = os.listdir(fileDir)
        # // 5. Loop through the files
        # index = 0
        # for file in files:
        #     # // 6. Get the file path
        #     filePath = os.path.join(fileDir, file)
        #     # // 7. Get the file extension
        #     fileExtension = os.path.splitext(filePath)[1]

        #     if fileExtension == ".png":
        #         # // 8. Rename the file
        #         os.rename(filePath, os.path.join(fileDir, subFolder + str(index) + fileExtension))
        #         index += 1
            
    