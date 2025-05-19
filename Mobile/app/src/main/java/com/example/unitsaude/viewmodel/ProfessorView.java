package com.example.unitsaude.viewmodel;

import android.app.Application;
import android.util.Log;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import com.example.unitsaude.data.api.ApiService;
import com.example.unitsaude.data.api.ApiClient;

import com.example.unitsaude.data.dto.professor.GetProfessorResponse;

import com.example.unitsaude.data.repositorio.ProfessorRepository;
import com.example.unitsaude.utils.SharedPreferencesManager;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ProfessorView extends AndroidViewModel {
    private final MutableLiveData<String> errorLiveData = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loadingLiveData = new MutableLiveData<>(false);
    private final MutableLiveData<GetProfessorResponse> professorResponseLiveData = new MutableLiveData<>();
    private final ProfessorRepository professorRepository;

    public ProfessorView(Application application) {
        super(application);
        professorRepository = new ProfessorRepository();
    }

    public LiveData<String> getErrorLiveData() {
        return errorLiveData;
    }

    public LiveData<Boolean> getLoadingLiveData() {
        return loadingLiveData;
    }

    public LiveData<GetProfessorResponse> getProfessorResponseLiveData() {
        return professorResponseLiveData;
    }

    public void getProfessoresPorEspecialidade(String especialidade) {
        loadingLiveData.setValue(true);

        professorRepository.getProfessoresPorEspecialidade(especialidade)
            .enqueue(new Callback<GetProfessorResponse>() {
                @Override
                public void onResponse(Call<GetProfessorResponse> call, Response<GetProfessorResponse> response) {
                    loadingLiveData.setValue(false);
                    if (response.isSuccessful() && response.body() != null) {
                        handleSuccessfulProfessores(response.body());
                    } else {
                        handleProfessoresError(response);
                    }
                }

                @Override
                public void onFailure(Call<GetProfessorResponse> call, Throwable t) {
                    loadingLiveData.setValue(false);
                    errorLiveData.setValue("Erro ao conectar com o servidor");
                    Log.e("PROFESSOR_ERROR", "Erro ao conectar com o servidor", t);
                }
            });
    }

    private void handleSuccessfulProfessores(GetProfessorResponse body) {
        professorResponseLiveData.setValue(body);
    }

    private void handleProfessoresError(Response<GetProfessorResponse> response) {
        String errorBody = "";
        try {
            if (response.errorBody() != null) {
                errorBody = response.errorBody().string();
            }
        } catch (Exception e) {
            errorBody = "Erro ao ler o corpo de erro: " + e.getMessage();
        }
        Log.e("PROFESSOR_FAIL", "Código HTTP: " + response.code() + ", Erro: " + errorBody);
        if (response.code() == 404) {
            errorLiveData.setValue("Professores não encontrados.");
        } else {
            errorLiveData.setValue("Erro ao consultar: " + errorBody);
        }
    }
}
