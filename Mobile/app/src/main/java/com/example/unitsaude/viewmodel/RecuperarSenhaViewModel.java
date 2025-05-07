package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Log;

import androidx.annotation.NonNull;
import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.unitsaude.data.repositorio.AuthRepository;
import com.example.unitsaude.data.dto.auth.RecuperarSenhaResponse;

import java.io.IOException;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class RecuperarSenhaViewModel extends AndroidViewModel {

    private final AuthRepository authRepository;
    private final MutableLiveData<Boolean> loadingLiveData ;
    private final MutableLiveData<String> successLiveData;
    private final MutableLiveData<String> errorLiveData;

    public RecuperarSenhaViewModel(@NonNull Application application) {
        super(application);
        authRepository = new AuthRepository();
        errorLiveData = new MutableLiveData<>();
        successLiveData = new MutableLiveData<>();
        loadingLiveData = new MutableLiveData<>(false);
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<String> getSuccessLiveData() {
        return successLiveData;
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }


    public void recuperarSenha(String email) {
        loadingLiveData.setValue(true);

        authRepository.recuperarSenha(email).enqueue(new Callback<ResponseBody>() {
            @Override
            public void onResponse(Call<ResponseBody> call, Response<ResponseBody> response) {
                loadingLiveData.setValue(false);

                if (response.isSuccessful()) {
                    try {
                        if (response.body() != null) {
                            String message = response.body().string();
                            successLiveData.setValue(message);
                        } else {
                            errorLiveData.setValue("Resposta vazia do servidor.");
                        }
                    } catch (IOException e) {
                        errorLiveData.setValue("Erro ao processar a resposta.");
                        Log.e("RECUPERAR_SENHA_FAIL", "Erro ao ler a resposta: " + e.getMessage());
                    }
                } else {
                    String errorMessage = "Erro ao recuperar senha. Código: " + response.code();
                    try {
                        if (response.errorBody() != null) {
                            errorMessage += "\n" + response.errorBody().string();
                        }
                    } catch (IOException e) {
                        Log.e("RECUPERAR_SENHA_FAIL", "Erro ao ler erro: " + e.getMessage());
                    }
                    errorLiveData.setValue(errorMessage);
                }
            }

            @Override
            public void onFailure(Call<ResponseBody> call, Throwable t) {
                loadingLiveData.setValue(false);
                errorLiveData.setValue("Erro de conexão: " + t.getMessage());
                Log.e("RECUPERAR_SENHA_ERROR", "Erro ao conectar com o servidor", t);
            }
        });

    }


}
