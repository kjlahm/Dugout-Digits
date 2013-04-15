package com.dugoutdigits;

import com.dugoutdigits.accessors.LocalResourceAccessor;
import com.dugoutdigits.utilities.AppConstants;

import android.os.AsyncTask;
import android.os.Bundle;
import android.app.Activity;
import android.content.res.Resources;
import android.graphics.Typeface;
import android.util.Log;
import android.view.Menu;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.TextView;

public class RosterActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_roster);
		
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
		
		// Fire task to load the teams drop down
		new LoadTeams().execute();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.activity_roster, menu);
		return true;
	}
	
	/**
	 * Loads the teams from the local resource accessor.
	 */
	private class LoadTeams extends AsyncTask<Void, Void, String[]> {
		@Override
		protected String[] doInBackground(Void... params) {
			return LocalResourceAccessor.getInstance().getTeams();
		}
		
		@Override
		protected void onPostExecute(String[] result) {
			Spinner spinner = (Spinner) findViewById(R.id.roster_teamdropdown);
			ArrayAdapter<CharSequence> adapter = new ArrayAdapter<CharSequence>(spinner.getContext(), android.R.layout.simple_spinner_item, result);
			adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
			spinner.setAdapter(adapter);
		}
	}

}
