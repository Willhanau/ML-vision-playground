//
//  SaveToPhotoLibrary.cpp
//  SaveToPhotoLibrary
//
//  Created by William Hanau on 10/4/17.
//  Copyright © 2017 William Hanau. All rights reserved.
//

#import <UIKit/UIKit.h>

extern "C"{
    
    char* makeStringCopy (const char* string) {
        if (string == NULL) return NULL;
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
    void _savePhotoToPhone(char* imagePath){
        //char* converted_imagePath = makeStringCopy(imagePath);
        NSString* imageDataPath = [NSString stringWithUTF8String:imagePath];
        UIImage* image = [UIImage imageNamed:imageDataPath];
        UIImageWriteToSavedPhotosAlbum(image, NULL, NULL, NULL);
    }
    
}
