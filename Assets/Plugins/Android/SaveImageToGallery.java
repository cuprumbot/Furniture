package edu.galileo.innovacion.furniture;

import android.content.ContentValues;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Build;
import android.provider.MediaStore;
import android.net.Uri;
import android.util.Log;

import java.io.OutputStream;
import java.io.File;
import java.io.FileInputStream;

public class SaveImageToGallery {

    public static void Save(Context context, String imagePath) {
        try {
            File file = new File(imagePath);
            if (!file.exists()) {
                Log.e("SaveImageToGallery", "File does not exist: " + imagePath);
                return;
            }

            Bitmap bitmap = BitmapFactory.decodeStream(new FileInputStream(file));
            String filename = file.getName();

            ContentValues values = new ContentValues();
            values.put(MediaStore.Images.Media.DISPLAY_NAME, filename);
            values.put(MediaStore.Images.Media.MIME_TYPE, "image/png");
            values.put(MediaStore.Images.Media.RELATIVE_PATH, "Pictures/ARApp");

            Uri uri = context.getContentResolver().insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);
            OutputStream stream = context.getContentResolver().openOutputStream(uri);

            bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
            stream.close();

            Log.i("SaveImageToGallery", "Image saved to gallery: " + filename);
        } catch (Exception e) {
            Log.e("SaveImageToGallery", "Error saving image: " + e.getMessage(), e);
        }
    }
}
