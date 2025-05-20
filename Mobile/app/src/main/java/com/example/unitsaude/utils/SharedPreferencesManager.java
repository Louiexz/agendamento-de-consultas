package com.example.unitsaude.utils;

import android.content.Context;
import android.content.SharedPreferences;

import android.content.Context;
import android.content.SharedPreferences;

public class SharedPreferencesManager {

    private static final String PREF_NAME = "UnitSaudePreferences";
    private static final String KEY_AUTH_TOKEN = "auth_token";
    private static final String KEY_IS_LOGGED_IN = "isLoggedIn";
    private static final String KEY_USER_NOME = "user_nome";
    private static final String KEY_USER_TIPO = "user_tipo";
    private static final String KEY_USER_ID = "user_id";
    private static SharedPreferencesManager instance;
    private SharedPreferences sharedPreferences;

    public SharedPreferencesManager(Context context) {
        sharedPreferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    }
    public static SharedPreferencesManager getInstance(Context context) {
        if (instance == null) {
            instance = new SharedPreferencesManager(context.getApplicationContext());
        }
        return instance;
    }

    public void setLoginIn(boolean saveLogin) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putBoolean(KEY_IS_LOGGED_IN, saveLogin);
        editor.apply();
    }
    public boolean isLoggedIn() {
        return sharedPreferences.getBoolean(KEY_IS_LOGGED_IN, false);
    }

    public void saveAuthToken(String token) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(KEY_AUTH_TOKEN, token);
        editor.apply();  // Salva de forma assíncrona
    }

    public void saveUserInfo(String nome, String tipoUsuario, int idUsuario) {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(KEY_USER_NOME, nome);
        editor.putString(KEY_USER_TIPO, tipoUsuario);
        editor.putInt(KEY_USER_ID, idUsuario);
        editor.apply();
    }

    public String getUserNome() {
        return sharedPreferences.getString(KEY_USER_NOME, null);
    }

    public String getUserTipo() {
        return sharedPreferences.getString(KEY_USER_TIPO, null);
    }

    public int getUserId() {
        return sharedPreferences.getInt(KEY_USER_ID, -1);
    }    

    public String getAuthToken() {
        return sharedPreferences.getString(KEY_AUTH_TOKEN, null);
    }

    public void clearUserInfo() {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.remove(KEY_USER_NOME);
        editor.remove(KEY_USER_TIPO);
        editor.remove(KEY_USER_ID);
        editor.apply();
    }

    public void clearLogin() {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.remove(KEY_IS_LOGGED_IN);
        editor.apply();
    }

    public void clearAuthToken() {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.remove(KEY_AUTH_TOKEN);
        editor.apply();
    }

    public void clearAll() {
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.clear();
        editor.apply();
    }
}

