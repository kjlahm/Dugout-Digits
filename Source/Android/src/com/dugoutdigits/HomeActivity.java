package com.dugoutdigits;

import com.dugoutdigits.accessors.LocalResourceAccessor;
import com.dugoutdigits.adapters.HomeIconAdapter;
import com.dugoutdigits.utilities.AppConstants;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Typeface;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.GridView;
import android.widget.TextView;
import android.widget.Toast;

public class HomeActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_home);
		
		// Attempt to set the font of the title in the action bar
		try {
			TextView title;
			Typeface  type = Typeface.createFromAsset(getAssets(),"fonts/Marcsc__.ttf");
			int titleId = Resources.getSystem().getIdentifier("action_bar_title", "id", "android");
		    title = (TextView) findViewById(titleId);
		    title.setTextSize(AppConstants.ACTION_BAR_TITLE_TEXTSIZE);
		    title.setTypeface(type);
		} catch (Exception ex) {
			Log.e("DD-HomeActivity", "Couldn't change action bar title font.");
		}
		
		// Fill the gridview with icons
		GridView gridview = (GridView) findViewById(R.id.home_gridview);
	    gridview.setAdapter(new HomeIconAdapter(this));

	    // Register click handler for the icons
	    gridview.setOnItemClickListener(new OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, View v, int position, long id) {
				switch (position) {
					case 0:
						Toast.makeText(HomeActivity.this, "" + position, Toast.LENGTH_SHORT).show();
						break;
	            	case 1:
	            		Toast.makeText(HomeActivity.this, "" + position, Toast.LENGTH_SHORT).show();
	            		break;
	            	case 2:
	            		startActivity(new Intent(v.getContext(), RosterActivity.class));
	            		break;
	            	case 3:
	            		Toast.makeText(HomeActivity.this, "" + position, Toast.LENGTH_SHORT).show();
	            		break;
	            	default:
	            		Toast.makeText(HomeActivity.this, "Unrecognized click.", Toast.LENGTH_SHORT).show();
	            		break;
				}
			}
	    });
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		if (LocalResourceAccessor.getInstance().IsAuthenticated(getApplicationContext())) {
			getMenuInflater().inflate(R.menu.activity_home_auth, menu);
		} else {
			getMenuInflater().inflate(R.menu.activity_home, menu);
		}
		return true;
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    // Handle item selection
	    switch (item.getItemId()) {
	        case R.id.menu_login:
	        	startActivity(new Intent(getApplicationContext(), LoginActivity.class));
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}

}
