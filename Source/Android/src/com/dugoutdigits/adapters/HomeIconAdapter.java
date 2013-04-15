package com.dugoutdigits.adapters;

import android.content.Context;
import android.graphics.drawable.Drawable;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.GridView;
import android.widget.TextView;

public class HomeIconAdapter extends BaseAdapter {
    private Context mContext;

    public HomeIconAdapter(Context c) {
        mContext = c;
    }

    public int getCount() {
        return mThumbIds.length;
    }

    public Object getItem(int position) {
        return null;
    }

    public long getItemId(int position) {
        return 0;
    }

    // create a new ImageView for each item referenced by the Adapter
    public View getView(int position, View convertView, ViewGroup parent) {
        TextView textView;
        if (convertView == null) {  // if it's not recycled, initialize some attributes
            textView = new TextView(mContext);
            textView.setLayoutParams(new GridView.LayoutParams(225, 275));
            //textView.setScaleType(ImageView.ScaleType.CENTER_CROP);
            textView.setPadding(10, 30, 10, 15);
        } else {
            textView = (TextView) convertView;
        }

        Drawable draw = mContext.getResources().getDrawable( mThumbIds[position] );
        draw.setBounds(0, 0, 200, 200);
        textView.setCompoundDrawables(null, draw, null, null);
        textView.setText(mTitleIds[position]);
        textView.setGravity(Gravity.CENTER_HORIZONTAL);
        return textView;
    }

    // references to our titles
    private Integer[] mTitleIds = {
    		com.dugoutdigits.R.string.home_addgame_title,
    		com.dugoutdigits.R.string.home_gamelibrary_title,
    		com.dugoutdigits.R.string.home_manageroster_title,
    		com.dugoutdigits.R.string.home_practicefilm_title
    };
    
    // references to our images
    private Integer[] mThumbIds = {
    		com.dugoutdigits.R.drawable.add_game,
    		com.dugoutdigits.R.drawable.library,
    		com.dugoutdigits.R.drawable.team,
    		com.dugoutdigits.R.drawable.film
    };
}
