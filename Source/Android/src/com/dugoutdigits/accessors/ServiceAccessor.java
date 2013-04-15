package com.dugoutdigits.accessors;

import java.io.IOException;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;
import org.json.JSONException;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.widget.Toast;

public class ServiceAccessor {
	private Context context;
	public ServiceAccessor(Context c) {
		context = c;
	}
	
	/*
	 * Service URLs
	 */
	private final String URL_BASE = "http://www.dugoutdigits.com/API/";
	
	private final String URL_LOGON = "Login";
	
	/*
	 * Return classes for service accessor calls.
	 */
	public class ServiceCall {
		public LogonResult success;
	}
	
	public enum LogonResult  { SUCCESS, USER_NOT_FOUND, BAD_PASSWORD, ERROR }
	public class LogonResponse extends ServiceCall {
		public String authToken;
	}
	
	/*
	 * Accessor calls
	 */
	public LogonResponse Logon(String username, String password) {
		LogonResponse response = new LogonResponse();
		response.success = LogonResult.SUCCESS;
		
		try {
			// Make the call to the API
			String jsonParam = String.format("{\"login\": {\"Username\": \"%s\", \"Password\": \"%s\"}", username, password);
			String callResponse = this.makeApiCallPost(URL_LOGON, jsonParam);
		    
		    // Parse the JSON
			Toast.makeText(context, callResponse, Toast.LENGTH_SHORT).show();
		} catch (Exception ex) {
			response.success = LogonResult.ERROR;
		}
		
		return response;
	}
	
	/*
	 * Helper Methods
	 */
	private String makeApiCallPost(String url, String jsonString) throws ClientProtocolException, IOException, JSONException {
		DefaultHttpClient httpClient = new DefaultHttpClient();
		
	    String uri = this.URL_BASE + url;

	    HttpPost postMethod = new HttpPost(uri);
	    postMethod.setEntity(new StringEntity(jsonString, "utf-8"));

	    HttpResponse response = httpClient.execute(postMethod);
	    return EntityUtils.toString(response.getEntity(), "utf-8");
	}
	
	private boolean isNetworkAvailable() {
		boolean outcome = false;
		if (context != null) {
			ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);

			NetworkInfo[] networkInfos = cm.getAllNetworkInfo();

			for (NetworkInfo tempNetworkInfo : networkInfos) {
				/**
				 * Can also check if the user is in roaming
				 */
				if (tempNetworkInfo.isConnected()) {
					outcome = true;
					break;
				}
			}
		}
        return outcome;
    }
}
