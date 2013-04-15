package com.dugoutdigits.accessors;

import android.content.Context;
import android.content.SharedPreferences;

public class LocalResourceAccessor {
	private static final String PREFS_NAME = "DugoutDigitsPrefFile";
	
	private static final String PREF_AUTHENTICATED = "DD_authenticated";
	private static final String PREF_AUTH_TOKEN = "DD_authToken";
	
	private static class Holder {
	    public static LocalResourceAccessor instance = new LocalResourceAccessor();
	}

	public static LocalResourceAccessor getInstance() {
	    return Holder.instance;
	}
	
	public String[] getTeams() {
	    String[] teams = {"New York Yankees","Boston Redsox","Baltimore Orioles","Tampa Bay Rays","Toronto Bluejays"};
	    return teams;
	}
	
	public boolean IsAuthenticated(Context context) {
		boolean authenticated = false;
		try {
			SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
			authenticated = settings.getBoolean(PREF_AUTHENTICATED, false);
		} catch (Exception ex) {
		}
		return authenticated;
	}
	
	public boolean SetAuthenticated(Context context, boolean authenticated) {
		boolean success = true;
		try {
			SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
			SharedPreferences.Editor editor = settings.edit();
			editor.putBoolean(PREF_AUTHENTICATED, authenticated);
			editor.commit();
		} catch (Exception ex) {
			success = false;
		}
		return success;
	}
	
	public boolean SaveAuthToken(Context context, String authToken) {
		boolean success = true;
		try {
			SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
			SharedPreferences.Editor editor = settings.edit();
			editor.putString(PREF_AUTH_TOKEN, authToken);
			editor.commit();
		} catch (Exception ex) {
			success = false;
		}
		return success;
	}
	
	public String GetAuthToken(Context context) {
		String authToken = "";
		try {
			SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
			authToken = settings.getString(PREF_AUTH_TOKEN, "");
		} catch (Exception ex) {
		}
		return authToken;
	}
}
